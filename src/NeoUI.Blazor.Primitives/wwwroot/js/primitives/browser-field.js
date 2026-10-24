import { canInvokeServer } from './browser-connection.js';
import { attachTags } from './browser-tags.js';
import { attachSegments } from './browser-segments.js';
// Form-aware data controls share one optimistic queue. Business acceptance is
// versioned independently of local checked/pressed/drag feedback.
const instances = new Map(), fieldKey = Symbol.for('NeoUI.BrowserField');
let removals;
const key = values => JSON.stringify(values);
export function initialize(root, bridge) {
    if (!root?.isConnected) return null;
    if (instances.has(root)) return instances.get(root);
    let config = JSON.parse(root.dataset.neoStateHost), version = config.version, values = [...config.values];
    let lastSubmittedSequence = -1;
    let accepted = key(values), sequence = 0, generation = 0, disposed = false, running = false, composing = false, locked = 0, timer, dragging;
    let editor, observer, dragStart;
    const observe = () => observer?.observe(root,{attributes:true,attributeFilter:['data-neo-state-host'],childList:true,subtree:true});
    const queue = [], waiters = new Set(), doc = root.ownerDocument;
    const rangeRtl = () => getComputedStyle(root.querySelector('[data-neo-field-range]') ?? root).direction === 'rtl';
    const own = element => element.closest('[data-neo-browser-field]') === root;
    const items = () => [...root.querySelectorAll('[data-neo-field-item]')].filter(own);
    const disabled = item => config.disabled || item?.matches(':disabled,[data-disabled="true"]');
    const emit = () => root.dispatchEvent(new CustomEvent('neoui:field-state',{bubbles:true,detail:{values,sequence,version}}));
    const settle = () => {
        for (const waiter of [...waiters]) {
            const status = disposed ? 'Disposed' : waiter.version !== version ? 'Reset' : waiter.sequence !== sequence ? 'Changed' : root.hasAttribute('data-neo-field-uncertain') ? 'Uncertain' : !root.hasAttribute('data-neo-field-pending') ? (accepted === waiter.value ? 'Success' : 'ValueAdjusted') : null;
            if (status) { clearTimeout(waiter.timer); waiters.delete(waiter); waiter.resolve({status,fieldId:root.id}); }
        }
    };
    const paint = () => {
        observer?.disconnect();
        const radio = config.kind === 'radio' || config.kind === 'togglegroup' && !config.multiple;
        const all = items(), enabled = all.filter(item=>!disabled(item));
        const roving = enabled.find(item=>values.includes(item.dataset.neoFieldItem)) ?? enabled[0];
        for (const item of all) {
            const checked = values.includes(item.dataset.neoFieldItem), mixed = values.includes('mixed');
            if (config.kind === 'toggle') item.setAttribute('aria-pressed',String(checked));
            else item.setAttribute('aria-checked',mixed ? 'mixed' : String(checked));
            item.dataset.state = mixed ? 'indeterminate' : checked ? (config.kind === 'toggle' ? 'on' : 'checked') : (config.kind === 'toggle' ? 'off' : 'unchecked');
            if (radio) item.tabIndex = item === roving ? 0 : -1;
            for (const child of item.querySelectorAll('[data-neo-field-state]')) child.dataset.state = item.dataset.state;
            // SVGElement has no reflected hidden property; use the attribute for both SVG and HTML indicators.
            for (const child of item.querySelectorAll('[data-neo-field-indicator]')) child.toggleAttribute('hidden', child.dataset.neoFieldIndicator !== (mixed ? 'mixed' : checked ? 'checked' : 'unchecked'));
        }
        for (const hidden of root.querySelectorAll('[data-neo-field-hidden]')) if (own(hidden)) hidden.value = config.kind === 'checkbox' || config.kind === 'switch' ? String(values.includes('checked')) : config.kind === 'radio' && values.length ? String(JSON.parse(values[0])) : values.join(',');
        const range = root.querySelector('[data-neo-field-range]');
        if (range) {
            const [low,high] = values.map(Number), options = config.fieldOptions ?? {}, min = options.min ?? 0, max = options.max ?? 100, rtl = !options.vertical && rangeRtl();
            for (const input of root.querySelectorAll('input[type="range"]')) input.value = values[Number(input.dataset.neoFieldIndex ?? 0)] ?? String(min);
            const percentage = value => max === min ? 0 : (value-min)/(max-min)*100;
            if (range.matches('input[type="range"]')) range.style.background = `linear-gradient(to ${rtl?'left':'right'}, var(--primary) ${percentage(low)}%, var(--input) ${percentage(low)}%)`;
            const fill = root.querySelector('[data-neo-field-fill]');
            if (fill) { fill.style[options.vertical?'bottom':'left'] = `${rtl?100-percentage(high):percentage(low)}%`; fill.style[options.vertical?'height':'width'] = `${percentage(high)-percentage(low)}%`; }
            for (const thumb of root.querySelectorAll('[data-neo-field-thumb]')) {
                const value = values[Number(thumb.dataset.neoFieldThumb)]; thumb.setAttribute('aria-valuenow',value);
                thumb.style[options.vertical ? 'bottom' : 'left'] = `calc(${rtl?100-percentage(Number(value)):percentage(Number(value))}% - ${options.half ?? 0}px)`;
                thumb.setAttribute('aria-valuemin',String(Number(thumb.dataset.neoFieldThumb)===1?low:min));thumb.setAttribute('aria-valuemax',String(Number(thumb.dataset.neoFieldThumb)===0?high:max));
            }
        }
        if (config.kind === 'otp') {
            const value=values[0] ?? '';
            for (const slot of root.querySelectorAll('[data-neo-otp-slot]')) {
                const index=Number(slot.dataset.neoOtpSlot);slot.value=value[index]??'';
                slot.dataset.filled=String(Boolean(value[index]));slot.dataset.active=String(doc.activeElement===slot);
            }
        }
        for (const tip of root.querySelectorAll('[data-neo-field-tooltip]')) tip.textContent = values[Number(tip.dataset.neoFieldTooltip)] ?? '';
        editor?.paint();
        observe();settle(); emit();
    };
    async function pump() {
        if (running || disposed) return; running = true; const run = generation;
        try {
            while (queue.length && !disposed && run === generation) {
                const edit = queue.shift();
                try {
                    if (config.server && !canInvokeServer()) { queue.unshift(edit); break; }
                    const ack = await bridge.invokeMethodAsync('ChangeAsync',edit.values,edit.sequence,edit.version);
                    if (disposed || run !== generation || edit.version !== version || edit.sequence !== sequence) continue;
                    if (ack.ignored) throw new Error('Uncertain acceptance');
                    values = [...ack.values]; accepted = key(values); root.removeAttribute('data-neo-field-pending'); paint();
                } catch {
                    if (!disposed && run === generation) { queue.length=0; root.dataset.neoFieldUncertain='true'; settle(); emit(); }
                }
            }
        } finally { if (run === generation) running=false; }
    }
    const notify = () => {
        clearTimeout(timer);
        if (!config.notify || !bridge) { accepted=key(values); root.removeAttribute('data-neo-field-pending');paint();return; }
        if (lastSubmittedSequence === sequence) return;
        lastSubmittedSequence = sequence;
        queue.push({values:[...values],sequence,version}); void pump();
    };
    const changed = (next, commit = true) => {
        if (locked || config.disabled || key(next) === key(values)) return;
        values=next;sequence++;delete root.dataset.neoFieldUncertain;root.dataset.neoFieldPending='true';paint();
        if (commit) notify();
    };
    const activate = item => {
        if (!item || disabled(item) || locked) return;
        const value=item.dataset.neoFieldItem;
        if (values.includes('mixed')) changed(['checked']);
        else if (values.includes(value)) { if (config.kind !== 'radio' && (config.collapsible || config.multiple)) changed(values.filter(item=>item!==value)); }
        else changed(config.multiple ? [...values,value] : [value]);
    };
    const click = event => {
        const item=event.target.closest('[data-neo-field-item]');if(!items().includes(item))return;
        event.preventDefault();event.stopImmediatePropagation();activate(item);
    };
    const otpSlots = () => [...root.querySelectorAll('[data-neo-otp-slot]')];
    const editOtp = (slot,text) => {
        if (config.kind !== 'otp' || locked || config.disabled || composing) return;
        const index=Number(slot.dataset.neoOtpSlot),value=values[0]??'';
        let pattern;try{pattern=new RegExp(config.fieldOptions.pattern??'[0-9]');}catch{pattern=/[\s\S]/;}
        const clean=[...text].filter(char=>pattern.test(char)).join('');
        const next=(value.slice(0,index)+clean+value.slice(index+(clean.length||1))).slice(0,config.fieldOptions.length);
        changed([next]);otpSlots()[Math.min(index+clean.length,config.fieldOptions.length-1)]?.focus();
    };
    const rangeValue=(index,value,commit=true)=>{
        const options=config.fieldOptions,step=Math.max(Number.EPSILON,options.step??1),next=[...values];
        value=Math.round((value-options.min)/step)*step+options.min;
        value=Math.max(index===1?Number(next[0]):options.min,Math.min(index===0?Number(next[1]):options.max,value));
        next[index]=String(Number(value.toPrecision(14)));changed(next,commit);
    };
    const keydown = event => {
        if(config.kind==='range' && event.target.matches('[data-neo-field-thumb]') && !locked && !config.disabled) {
            const index=Number(event.target.dataset.neoFieldThumb),current=Number(values[index]),options=config.fieldOptions;
            const horizontalSign=!options.vertical&&rangeRtl()?-1:1;
            const steps={ArrowRight:horizontalSign,ArrowUp:1,ArrowLeft:-horizontalSign,ArrowDown:-1,PageUp:10,PageDown:-10};
            if(event.key in steps||['Home','End'].includes(event.key)){event.preventDefault();event.stopImmediatePropagation();rangeValue(index,event.key==='Home'?options.min:event.key==='End'?options.max:current+steps[event.key]*options.step);return;}
        }
        if (config.kind === 'otp' && event.target.matches('[data-neo-otp-slot]') && !event.isComposing && !locked && !config.disabled) {
            const slots=otpSlots(),index=slots.indexOf(event.target);
            if (['ArrowLeft','ArrowRight','Home','End'].includes(event.key)) {
                event.preventDefault();slots[event.key==='Home'?0:event.key==='End'?slots.length-1:Math.max(0,Math.min(slots.length-1,index+(event.key==='ArrowRight'?1:-1)))]?.focus();return;
            }
            if (['Backspace','Delete'].includes(event.key)) {
                event.preventDefault();const at=event.key==='Backspace'&&!event.target.value?Math.max(0,index-1):index;
                editOtp(slots[at],'');slots[at]?.focus();return;
            }
        }
        const item=event.target.closest('[data-neo-field-item]');if(!items().includes(item)||event.isComposing||disabled(item)||locked)return;
        if (!['radio','togglegroup'].includes(config.kind) || !['ArrowLeft','ArrowRight','ArrowUp','ArrowDown','Home','End'].includes(event.key)) return;
        event.preventDefault();event.stopImmediatePropagation();
        const list=items().filter(item=>!disabled(item)),index=list.indexOf(item),rtl=getComputedStyle(root).direction==='rtl';
        const forward=event.key==='ArrowDown'||event.key===(rtl?'ArrowLeft':'ArrowRight');
        const next=event.key==='Home'?list[0]:event.key==='End'?list.at(-1):list[(index+(forward?1:-1)+list.length)%list.length];
        next?.focus();if(!config.multiple)activate(next);else for(const trigger of list)trigger.tabIndex=trigger===next?0:-1;
    };
    const input = event => {
        if (config.kind === 'otp' && event.target.matches('[data-neo-otp-slot]')) { editOtp(event.target,event.target.value);return; }
        if (!event.target.matches('input[type="range"]') || !own(event.target)) return;
        if (locked || config.disabled) {paint();return;}
        const next=[...values],index=Number(event.target.dataset.neoFieldIndex??0);next[index]=event.target.value;
        if(next.length===2){if(index===0)next[0]=String(Math.min(Number(next[0]),Number(next[1])));else next[1]=String(Math.max(Number(next[0]),Number(next[1])));}
        changed(next,(config.fieldOptions?.updateOn ?? 'change')==='input');
    };
    const commit = event => {if(event.target.matches('input[type="range"]')&&own(event.target)){input(event);if(root.hasAttribute('data-neo-field-pending'))notify();}};
    const rangeRatio=event=>{
        const options=config.fieldOptions,rect=root.querySelector('[data-neo-field-range]').getBoundingClientRect();
        return options.vertical?(rect.bottom-event.clientY)/rect.height:rangeRtl()?(rect.right-event.clientX)/rect.width:(event.clientX-rect.left)/rect.width;
    };
    const moveRange=event=>{
        if(dragging===undefined||locked||config.disabled)return;
        const options=config.fieldOptions,ratio=rangeRatio(event);
        rangeValue(dragging,options.min+Math.max(0,Math.min(1,ratio))*(options.max-options.min),false);
    };
    const pointerDown=event=>{
        if(config.kind!=='range'||locked||config.disabled||event.button!==0||!event.target.closest('[data-neo-field-range]'))return;
        const thumb=event.target.closest('[data-neo-field-thumb]');
        if(thumb)dragging=Number(thumb.dataset.neoFieldThumb);
        else {const options=config.fieldOptions;const target=options.min+rangeRatio(event)*(options.max-options.min);dragging=Math.abs(target-Number(values[0]))<=Math.abs(target-Number(values[1]))?0:1;}
        dragStart=[...values];event.preventDefault();root.querySelector(`[data-neo-field-thumb="${dragging}"]`)?.focus();moveRange(event);
        root.setPointerCapture(event.pointerId);
    };
    const pointerUp=event=>{if(dragging!==undefined){moveRange(event);dragging=undefined;if(root.hasPointerCapture(event.pointerId))root.releasePointerCapture(event.pointerId);if(root.hasAttribute('data-neo-field-pending'))notify();}};
    const pointerCancel=event=>{if(dragging===undefined)return;dragging=undefined;if(root.hasPointerCapture(event.pointerId))root.releasePointerCapture(event.pointerId);if(dragStart){values=dragStart;dragStart=null;sequence++;if(accepted===key(values))delete root.dataset.neoFieldPending;paint();}};
    const beforeInput=event=>{if(locked)event.preventDefault();};
    const paste = event => { if(config.kind==='otp' && event.target.matches('[data-neo-otp-slot]')) {event.preventDefault();editOtp(event.target,event.clipboardData.getData('text'));} };
    const compositionStart = () => composing=true;
    const compositionEnd = event => {composing=false;input(event);};
    const focus = () => {if(config.kind==='otp')paint();};
    editor=attachSegments(root,()=>config,()=>values,changed,()=>locked>0) ?? attachTags(root,()=>config,()=>values,changed,()=>locked>0,bridge,()=>version);
    observer=new MutationObserver(()=>{
        const next=JSON.parse(root.dataset.neoStateHost),reset=next.version!==version;
        if(reset){values=[...next.values];version=next.version;generation++;running=false;queue.length=0;accepted=key(values);delete root.dataset.neoFieldPending;delete root.dataset.neoFieldUncertain;}
        config=next;if(reset)editor?.reset?.();paint();
    });
    observe();
    root.addEventListener('click',click,true);root.addEventListener('keydown',keydown,true);root.addEventListener('input',input,true);root.addEventListener('change',commit,true);root.addEventListener('paste',paste,true);root.addEventListener('beforeinput',beforeInput,true);root.addEventListener('pointerdown',pointerDown);root.addEventListener('pointermove',moveRange);root.addEventListener('pointerup',pointerUp);root.addEventListener('pointercancel',pointerCancel);root.addEventListener('focusin',focus);root.addEventListener('focusout',focus);root.addEventListener('compositionstart',compositionStart);root.addEventListener('compositionend',compositionEnd);
    const controller={focus(){if(config.disabled)return;if(editor?.focus)editor.focus();else root.querySelector('[data-neo-otp-slot],input:not(:disabled),[data-neo-field-item]:not(:disabled),[data-neo-field-thumb]')?.focus();},get disabled(){return config.disabled;},get composing(){return composing;},enableNativeValidation(){return ()=>{};},
        lock(){editor?.beforeFlush?.();locked++;return()=>{locked--;};},
        validate(){if(editor && !editor.validate())return {status:'Invalid',fieldId:root.id};if(config.kind==='otp' && (values[0]?.length ?? 0)!==config.fieldOptions.length && (config.fieldOptions.required || values[0]?.length))return {status:'Invalid',fieldId:root.id};return config.fieldOptions?.required && (['checkbox','switch'].includes(config.kind) ? !values.includes('checked') : !values.length) ? {status:'Invalid',fieldId:root.id} : {status:'Success'};},
        flush(timeout){
            if(config.server&&!canInvokeServer())return Promise.resolve({status:'Uncertain',fieldId:root.id});
            if(editor?.validateDraft && !editor.validateDraft())return Promise.resolve({status:'Invalid',fieldId:root.id});
            if(root.hasAttribute('data-neo-field-uncertain'))return Promise.resolve({status:'Uncertain',fieldId:root.id});
            return new Promise(resolve=>{const waiter={resolve,sequence,version,value:key(values)};waiters.add(waiter);waiter.timer=setTimeout(()=>{waiters.delete(waiter);root.dataset.neoFieldUncertain='true';resolve({status:'TimedOut',fieldId:root.id});},timeout);if(root.hasAttribute('data-neo-field-pending')&&!running&&!queue.length)notify();void pump();settle();});
        },
        dispose(){if(disposed)return;disposed=true;editor?.dispose();generation++;clearTimeout(timer);observer.disconnect();queue.length=0;settle();
            root.removeEventListener('click',click,true);root.removeEventListener('keydown',keydown,true);root.removeEventListener('input',input,true);root.removeEventListener('change',commit,true);root.removeEventListener('paste',paste,true);root.removeEventListener('beforeinput',beforeInput,true);root.removeEventListener('pointerdown',pointerDown);root.removeEventListener('pointermove',moveRange);root.removeEventListener('pointerup',pointerUp);root.removeEventListener('pointercancel',pointerCancel);root.removeEventListener('focusin',focus);root.removeEventListener('focusout',focus);root.removeEventListener('compositionstart',compositionStart);root.removeEventListener('compositionend',compositionEnd);delete root[fieldKey];instances.delete(root);
            if(!instances.size){removals?.disconnect();removals=null;}
        }
    };
    root[fieldKey]=controller;instances.set(root,controller);paint();root.dataset.neoStateReady='true';
    if(!removals){removals=new MutationObserver(()=>{for(const [root,instance]of instances)if(!root.isConnected)instance.dispose();});removals.observe(doc.body,{childList:true,subtree:true});}
    return controller;
}
