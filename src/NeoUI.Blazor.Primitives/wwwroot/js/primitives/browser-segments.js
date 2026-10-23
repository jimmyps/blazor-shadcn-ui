export function attachSegments(root, getConfig, getValues, change, isLocked) {
    if (!['date','time'].includes(getConfig().kind)) return null;
    const doc=root.ownerDocument, parts=()=>[...root.querySelectorAll('[data-neo-segment]')].filter(part=>part.closest('[data-neo-browser-field]')===root);
    let buffer='',bufferIndex=-1,lastDigit=0,blurTimer;
    const options=()=>getConfig().fieldOptions, descriptors=()=>options().segments;
    const fullYear=(year,segment)=>{
        if(segment.length>=4)return year;
        const max=options().twoDigitYearMax??2049;return Math.floor(max/100)*100+year-(year>max%100?100:0);
    };
    const valueByKind=(kind,values=getValues())=>{const index=descriptors().findIndex(segment=>segment.kind===kind);return index<0?null:values[index]===''?null:Number(values[index]);};
    const iso=()=>{
        const values=getValues();if(values.some(value=>value===''))return '';
        if(getConfig().kind==='date'){
            const desc=descriptors().find(segment=>segment.kind==='year'),year=fullYear(valueByKind('year'),desc);
            return `${String(year).padStart(4,'0')}-${String(valueByKind('month')).padStart(2,'0')}-${String(valueByKind('day')).padStart(2,'0')}`;
        }
        let hour=valueByKind('hour');const ampm=valueByKind('ampm');if(ampm!==null)hour=hour%12+(ampm===1?12:0);
        return `${String(hour).padStart(2,'0')}:${String(valueByKind('minute')).padStart(2,'0')}:${String(valueByKind('second')??0).padStart(2,'0')}`;
    };
    const paint=()=>{
        const values=getValues(),focused=parts().includes(doc.activeElement),empty=values.every(value=>value==='');
        const placeholder=root.querySelector('[data-neo-segment-placeholder]'),show=Boolean(placeholder)&&empty&&!focused;
        if(placeholder){placeholder.hidden=!show;placeholder.tabIndex=show&&!getConfig().disabled?0:-1;}
        parts().forEach((part,index)=>{
            const descriptor=descriptors()[index],value=values[index]??'';part.hidden=show;part.dataset.empty=String(value==='');
            part.textContent=value===''?descriptor.placeholder:descriptor.kind==='ampm'?(value==='0'?'AM':'PM'):value.padStart(descriptor.length,'0');
            if(value==='')part.removeAttribute('aria-valuenow');else part.setAttribute('aria-valuenow',value);
        });
        for(const literal of root.querySelectorAll('[data-neo-segment-literal]'))literal.hidden=show;
        for(const hidden of root.querySelectorAll('[data-neo-segment-hidden]'))hidden.value=iso();
    };
    const update=values=>{
        if(getConfig().kind==='date'){
            const year=valueByKind('year',values),month=valueByKind('month',values),day=valueByKind('day',values);
            if(year!==null&&month!==null&&day!==null){const full=fullYear(year,descriptors().find(segment=>segment.kind==='year'));const date=new Date(0);date.setFullYear(full,month,0);const maximum=date.getDate();const index=descriptors().findIndex(segment=>segment.kind==='day');values[index]=String(Math.min(day,maximum));}
        }
        change(values);
    };
    const seedNow=(values,overwrite=false,roundMinutes=false)=>{
        const now=new Date();
        descriptors().forEach((segment,index)=>{
            if(!overwrite&&values[index]!=='')return;
            let value={year:segment.length<4?now.getFullYear()%100:now.getFullYear(),month:now.getMonth()+1,day:now.getDate(),hour:segment.max===12?now.getHours()%12||12:now.getHours(),minute:now.getMinutes(),second:now.getSeconds(),ampm:now.getHours()>=12?1:0}[segment.kind];
            if(roundMinutes&&segment.kind==='minute'&&segment.step>1)value=Math.floor(value/segment.step)*segment.step;
            values[index]=String(value);
        });
    };
    const move=(index)=>{buffer='';bufferIndex=-1;parts()[Math.max(0,Math.min(parts().length-1,index))]?.focus();};
    const keydown=event=>{
        const part=event.target.closest('[data-neo-segment]');if(!parts().includes(part)||event.isComposing||event.ctrlKey||event.metaKey||event.altKey||isLocked()||getConfig().disabled)return;
        const index=Number(part.dataset.neoSegment),descriptor=descriptors()[index],values=[...getValues()];
        if(['ArrowLeft','ArrowRight','Home','End'].includes(event.key)){event.preventDefault();move(event.key==='Home'?0:event.key==='End'?parts().length-1:index+(event.key==='ArrowRight'?1:-1));return;}
        if(['ArrowUp','ArrowDown'].includes(event.key)){
            event.preventDefault();buffer='';
            const wasEmpty=values[index]==='';
            if(wasEmpty)seedNow(values,false,true);
            // AM/PM seeds from the clock on first activation; numeric segments
            // seed then increment, matching the existing Blazor implementation.
            if(descriptor.kind!=='ampm'||!wasEmpty){
                const step=descriptor.step??1,current=Number(values[index]),delta=event.key==='ArrowUp'?1:-1;
                const range=descriptor.max-descriptor.min+1;
                if(descriptor.kind==='minute'&&step>1){
                    const count=Math.max(1,Math.floor(range/step)),position=Math.floor((current-descriptor.min)/step);
                    values[index]=String(descriptor.min+((position+delta)%count+count)%count*step);
                }else values[index]=String(descriptor.min+((current-descriptor.min+delta)%range+range)%range);
            }
            update(values);return;
        }
        if(['Backspace','Delete'].includes(event.key)){event.preventDefault();buffer='';values[index]='';update(values);return;}
        if(descriptor.kind==='ampm'&&['a','p','A','P'].includes(event.key)){event.preventDefault();values[index]=event.key.toLowerCase()==='a'?'0':'1';update(values);return;}
        if(event.key.toLowerCase()==='c'){event.preventDefault();buffer='';seedNow(values,true);update(values);return;}
        if(descriptor.kind!=='ampm'&&/^\d$/.test(event.key)){
            event.preventDefault();if(bufferIndex!==index||Date.now()-lastDigit>1000)buffer='';bufferIndex=index;lastDigit=Date.now();buffer+=event.key;
            if(Number(buffer)>descriptor.max)buffer=event.key;
            const value=Number(buffer);values[index]=value<descriptor.min?'':String(value);update(values);
            if(buffer.length>=descriptor.length||value*10>descriptor.max&&value>=descriptor.min)move(index+1);
        }
    };
    const focus=event=>{
        buffer='';bufferIndex=-1;clearTimeout(blurTimer);
        // focusout runs before the next segment receives focus. Hiding empty
        // segments in that event would remove the browser's next Tab target.
        if(event.type==='focusout'){blurTimer=setTimeout(paint,0);return;}
        if(event.target.matches('[data-neo-segment-placeholder]')&&!getConfig().disabled){
            const placeholder=root.querySelector('[data-neo-segment-placeholder]');placeholder.hidden=true;
            parts().forEach(part=>part.hidden=false);parts()[0]?.focus();
        }
        paint();
    };
    const pointer=event=>{if(event.target.closest('[data-neo-segment-placeholder]')&&!getConfig().disabled) {event.preventDefault();const placeholder=root.querySelector('[data-neo-segment-placeholder]');placeholder.hidden=true;parts().forEach(part=>part.hidden=false);parts()[0]?.focus();}};
    root.addEventListener('keydown',keydown,true);root.addEventListener('focusin',focus);root.addEventListener('focusout',focus);root.addEventListener('click',pointer,true);
    return {paint,focus(){const placeholder=root.querySelector('[data-neo-segment-placeholder]');if(placeholder)placeholder.hidden=true;parts().forEach(part=>part.hidden=false);parts()[0]?.focus();},reset(){buffer='';bufferIndex=-1;},validate(){const values=getValues();return values.every(value=>value==='')?!options().required:values.every((value,index)=>value!==''&&Number(value)>=descriptors()[index].min&&Number(value)<=descriptors()[index].max);},dispose(){clearTimeout(blurTimer);root.removeEventListener('keydown',keydown,true);root.removeEventListener('focusin',focus);root.removeEventListener('focusout',focus);root.removeEventListener('click',pointer,true);}};
}
