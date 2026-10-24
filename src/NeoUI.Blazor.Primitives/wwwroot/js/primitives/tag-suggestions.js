// Shared gesture completion for portaled tag suggestions. Pointer-down never
// commits: a scroll, cancelled gesture or release outside the item is not a tap.
export function attachTagSuggestions(doc, getList, activate) {
    let pending=null, suppress=null;
    const itemAt=target=>target?.closest?.('[data-neo-tag-suggestion]');
    const own=item=>item && getList()?.contains(item);
    const down=event=>{
        pending=null;
        const item=itemAt(event.target);
        if(!own(item)||event.button!==0||event.isPrimary===false)return;
        if(event.pointerType==='touch'||event.pointerType==='pen'){
            pending={item,id:event.pointerId,x:event.clientX,y:event.clientY,cancelled:false};
        }else event.preventDefault(); // Keep mouse focus in the editor.
    };
    const move=event=>{if(pending?.id===event.pointerId && Math.hypot(event.clientX-pending.x,event.clientY-pending.y)>10)pending.cancelled=true;};
    const cancel=event=>{if(pending?.id===event.pointerId){suppress={item:pending.item,until:Date.now()+800};pending=null;}};
    const up=event=>{
        if(pending?.id!==event.pointerId)return;
        const gesture=pending;pending=null;suppress={item:gesture.item,until:Date.now()+800};
        const hit=doc.elementFromPoint(event.clientX,event.clientY);
        if(!gesture.cancelled&&own(gesture.item)&&gesture.item.contains(hit)){
            event.preventDefault();activate(gesture.item);
        }
    };
    const click=event=>{
        const item=itemAt(event.target);
        if(item===suppress?.item && Date.now()<suppress.until && (event.detail!==0||event.pointerType==='touch'||event.pointerType==='pen')){event.preventDefault();event.stopImmediatePropagation();return;}
        if(event.button===0&&own(item)){event.preventDefault();activate(item);}
    };
    doc.addEventListener('pointerdown',down,true);doc.addEventListener('pointermove',move,true);doc.addEventListener('pointerup',up,true);doc.addEventListener('pointercancel',cancel,true);doc.addEventListener('click',click,true);
    return ()=>{pending=suppress=null;doc.removeEventListener('pointerdown',down,true);doc.removeEventListener('pointermove',move,true);doc.removeEventListener('pointerup',up,true);doc.removeEventListener('pointercancel',cancel,true);doc.removeEventListener('click',click,true);};
}
