import { attachTagSuggestions } from './tag-suggestions.js';
import { canInvokeServer } from './browser-connection.js';
export function attachTags(root,getConfig,getValues,change,isLocked,bridge,getVersion) {
    if(getConfig().kind!=='tags')return null;
    const doc=root.ownerDocument,input=root.querySelector('[data-neo-tag-input]'),container=root.querySelector('[data-neo-tag-values]'),status=root.querySelector('[data-neo-tag-status]');
    let results=[],active=-1,timer,blurTimer,querySequence=0,disposed=false;
    const options=()=>getConfig().fieldOptions,list=()=>doc.getElementById(options().contentId);
    const reject=value=>{status.textContent=`Unable to add ${value}.`;if(!getConfig().server||canInvokeServer())void bridge?.invokeMethodAsync('RejectAsync',value,getVersion()).catch(()=>{});};
    const close=()=>{results=[];active=-1;if(list())list().hidden=true;input.setAttribute('aria-expanded','false');input.removeAttribute('aria-activedescendant');};
    const paintActive=()=>{
        const target=list();if(!target)return;
        // Keep the native touch/mouse target mounted throughout activation.
        for(const [index,item] of [...target.children].entries()){
            item.setAttribute('aria-selected',String(index===active));
            item.classList.toggle('bg-accent',index===active);item.classList.toggle('text-accent-foreground',index===active);
        }
        if(active>=0){input.setAttribute('aria-activedescendant',`${options().contentId}-${active}`);target.children[active]?.scrollIntoView({block:'nearest'});}else input.removeAttribute('aria-activedescendant');
    };
    const paintResults=()=>{
        const target=list();if(!target)return;target.replaceChildren();
        results.forEach((value,index)=>{const item=doc.createElement('li');item.id=`${options().contentId}-${index}`;item.role='option';item.dataset.neoTagSuggestion=String(index);item.textContent=value;item.setAttribute('aria-selected',String(index===active));item.className='flex cursor-pointer select-none items-center rounded-sm px-2 py-1.5 text-sm'+(index===active?' bg-accent text-accent-foreground':'');target.append(item);});
        target.hidden=!results.length;input.setAttribute('aria-expanded',String(results.length>0));
        paintActive();
    };
    const paint=()=>{
        const values=getValues(),focused=container.contains(doc.activeElement)?doc.activeElement:null;
        // Reuse badges by value/occurrence: acknowledgements must not detach a
        // focused remove button, including when duplicate tags are allowed.
        const available=new Map();
        for(const badge of container.children){const queue=available.get(badge._neoTagValue)??[];queue.push(badge);available.set(badge._neoTagValue,queue);}
        const retained=new Set();
        values.forEach((value,index)=>{
            let badge=available.get(value)?.shift();
            if(!badge){badge=doc.createElement('span');badge._neoTagValue=value;badge.append(doc.createTextNode(value));const button=doc.createElement('button');button.type='button';button.textContent='×';button.className='ml-0.5 rounded-full px-1';badge.append(button);}
            badge.className=options().badgeClass;
            const button=badge.lastElementChild;button.dataset.neoTagRemove=String(index);button.disabled=getConfig().disabled;button.setAttribute('aria-label',String(options().removeLabel??'Remove {tag}').replace('{tag}',value));
            retained.add(badge);if(container.children[index]!==badge)container.insertBefore(badge,container.children[index]??null);
        });
        for(const badge of [...container.children])if(!retained.has(badge))badge.remove();
        if(focused&&doc.activeElement!==focused&&!getConfig().disabled)(focused.isConnected?focused:input).focus({preventScroll:true});
        const clear=root.querySelector('[data-neo-tag-clear]');if(clear)clear.hidden=!values.length;
    };
    const addMany=texts=>{
        if(isLocked()||getConfig().disabled)return false;
        const values=[...getValues()];let added=false;
        for(const raw of texts){const value=raw.trim();if(!value)continue;
            if(value.length>options().maxLength||values.length>=options().maxTags||!options().duplicates&&values.some(other=>other.toLocaleLowerCase()===value.toLocaleLowerCase())){reject(value);continue;}
            values.push(value);added=true;
        }
        if(added){input.value='';status.textContent='';querySequence++;clearTimeout(timer);close();change(values);}
        return added;
    };
    const search=()=>{
        clearTimeout(timer);const query=input.value,sequence=++querySequence;close();
        if(!query.trim())return;
        const filter=values=>values.filter(value=>(options().duplicates||!getValues().some(tag=>tag.toLocaleLowerCase()===value.toLocaleLowerCase()))&&value.toLocaleLowerCase().includes(query.toLocaleLowerCase()));
        if(!options().search){results=filter(options().suggestions??[]);paintResults();return;}
        timer=setTimeout(async()=>{const version=getVersion();try{if(getConfig().server&&!canInvokeServer()){status.textContent='Suggestions are unavailable.';return;}const ack=await bridge.invokeMethodAsync('SearchAsync',query,version);if(disposed||sequence!==querySequence||getVersion()!==version||ack.ignored||input.value!==query)return;results=filter(ack.values);paintResults();}catch{if(sequence===querySequence)status.textContent='Suggestions are unavailable.';}},options().debounce??300);
    };
    const edited=event=>{if(event.target===input&&!event.isComposing)search();};
    const keydown=event=>{
        if(event.target!==input||event.isComposing||isLocked()||getConfig().disabled)return;
        if(['ArrowDown','ArrowUp'].includes(event.key)&&results.length){event.preventDefault();active=(active+(event.key==='ArrowDown'?1:-1)+results.length)%results.length;paintActive();return;}
        if(event.key==='Escape'){event.preventDefault();querySequence++;clearTimeout(timer);close();return;}
        const flags={Enter:1,',':2,' ':4,Tab:8,';':16};
        if(event.key==='Enter'&&active>=0){event.preventDefault();addMany([results[active]]);return;}
        if(flags[event.key]&&(options().triggers&flags[event.key])){if(event.key!=='Tab')event.preventDefault();addMany([input.value]);return;}
        if(event.key==='Backspace'&&!input.value&&getValues().length){event.preventDefault();change(getValues().slice(0,-1));}
    };
    const paste=event=>{
        if(event.target!==input||isLocked()||getConfig().disabled)return;
        const delimiters=[];if(options().triggers&2)delimiters.push(',');if(options().triggers&4)delimiters.push(' ');if(options().triggers&16)delimiters.push(';');
        const text=event.clipboardData.getData('text');if(delimiters.some(char=>text.includes(char))){event.preventDefault();addMany(text.split(new RegExp(`[${delimiters.join('')}\\n\\r]+`)));}
    };
    const click=event=>{
        const remove=event.target.closest('[data-neo-tag-remove]'),clear=event.target.closest('[data-neo-tag-clear]');
        if(remove&&root.contains(remove)){event.preventDefault();if(!isLocked()&&!getConfig().disabled)change(getValues().filter((value,index)=>index!==Number(remove.dataset.neoTagRemove)));input.focus();}
        if(clear&&root.contains(clear)){event.preventDefault();if(!isLocked()&&!getConfig().disabled)change([]);input.focus();}
        // Match the existing tag-input.js container click-to-focus behavior.
        if(root.contains(event.target)&&!event.target.closest('button')&&!getConfig().disabled)input.focus();
    };
    const pointer=event=>{
        const target=list();
        if(!root.contains(event.target)&&!target?.contains(event.target)){querySequence++;clearTimeout(timer);close();}
    };
    const detachSuggestions=attachTagSuggestions(doc,list,item=>{const value=results[Number(item.dataset.neoTagSuggestion)];if(value!==undefined&&!list()?.hidden){addMany([value]);input.focus();}});
    const hover=event=>{const item=event.target.closest('[data-neo-tag-suggestion]');if(list()?.contains(item)&&active!==Number(item.dataset.neoTagSuggestion)){active=Number(item.dataset.neoTagSuggestion);paintActive();}};
    const focusin=()=>clearTimeout(blurTimer);
    const focusout=()=>{
        clearTimeout(blurTimer);
        // Preserve the legacy grace period for a pointer selecting a suggestion.
        blurTimer=setTimeout(()=>{
            if(disposed||root.contains(doc.activeElement)||list()?.contains(doc.activeElement))return;
            querySequence++;clearTimeout(timer);close();
        },150);
    };
    root.addEventListener('focusin',focusin);root.addEventListener('focusout',focusout);
    root.addEventListener('input',edited);root.addEventListener('keydown',keydown,true);root.addEventListener('paste',paste,true);root.addEventListener('click',click,true);doc.addEventListener('pointerdown',pointer,true);doc.addEventListener('pointerover',hover);
    return {paint,validate:()=>!input.value.trim(),beforeFlush(){if(input.value.trim())addMany([input.value]);},reset(){input.value='';querySequence++;clearTimeout(timer);clearTimeout(blurTimer);close();},dispose(){disposed=true;querySequence++;clearTimeout(timer);clearTimeout(blurTimer);root.removeEventListener('focusin',focusin);root.removeEventListener('focusout',focusout);root.removeEventListener('input',edited);root.removeEventListener('keydown',keydown,true);root.removeEventListener('paste',paste,true);root.removeEventListener('click',click,true);doc.removeEventListener('pointerdown',pointer,true);detachSuggestions();doc.removeEventListener('pointerover',hover);}};
}
