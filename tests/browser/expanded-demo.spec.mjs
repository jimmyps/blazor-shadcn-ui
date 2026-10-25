import {test} from 'node:test';
import assert from 'node:assert/strict';
import {chromium} from 'playwright';
import {readFile} from 'node:fs/promises';
const url=process.env.NEOUI_DEMO_URL??'http://localhost:5184/components/browser-interactions';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
test('the expanded gallery uses existing styles, flushes mixed controls and fits desktop/mobile',async()=>{
    const browser=await chromium.launch({channel:'msedge',headless:true}),page=await browser.newPage({viewport:{width:1440,height:1100}}),errors=[];
    page.on('pageerror',error=>errors.push(String(error)));await page.addInitScript(floating);
    const origin=new URL(url).origin;
    if(origin.includes('.devtunnels.ms'))await page.route(origin+'/**',route=>route.continue({headers:{...route.request().headers(),'X-Tunnel-Skip-AntiPhishing-Page':'true'}}));
    try{
        await page.goto(url,{waitUntil:'domcontentloaded'});
        const heading=page.getByText('Data controls and editor drafts',{exact:true}),section=heading.locator('xpath=../..');
        const quantity=section.getByPlaceholder('Try a partial number',{exact:true}),tags=section.getByPlaceholder('Type a tag or paste a list',{exact:true}),markdown=section.getByPlaceholder('Type, select text, then try Ctrl+B or Ctrl+I',{exact:true});
        await quantity.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();await markdown.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await section.locator('[data-neo-browser-field][data-neo-state-ready]').first().waitFor();
        await section.getByRole('radio',{name:'Weekly digest',exact:true}).click();
        assert.equal(await section.getByRole('radio',{name:'Weekly digest',exact:true}).getAttribute('aria-checked'),'true');
        await quantity.fill('7');await tags.fill('Engineering');await markdown.fill('Newest expanded draft');await section.getByRole('button',{name:'Save controls',exact:true}).click();
        await section.getByText('Saved quantity 7, 1 tags and the latest editor draft.',{exact:true}).waitFor();
        assert.equal(await markdown.inputValue(),'Newest expanded draft');assert.match(await section.locator('[data-neo-tag-values]').textContent(),/Engineering/);
        await section.screenshot({path:process.env.TEMP+'/neoui-sweep-controls-desktop.png'});
        assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
        await page.setViewportSize({width:390,height:844});await heading.scrollIntoViewIfNeeded();assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
        await section.screenshot({path:process.env.TEMP+'/neoui-sweep-controls-mobile.png'});
        assert.deepEqual(errors,[]);
    }finally{await browser.close();}
});

test('delayed-binding gallery loads pristine values, preserves typed drafts and restarts safely',async()=>{
    const browser=await chromium.launch({channel:'msedge',headless:true}),page=await browser.newPage({viewport:{width:390,height:844}}),errors=[];
    page.on('pageerror',error=>errors.push(String(error)));await page.addInitScript(floating);
    const origin=new URL(url).origin;
    if(origin.includes('.devtunnels.ms'))await page.route(origin+'/**',route=>route.continue({headers:{...route.request().headers(),'X-Tunnel-Skip-AntiPhishing-Page':'true'}}));
    try{
        await page.goto(url,{waitUntil:'domcontentloaded'});
        const section=page.getByTestId('demo-delayed-binding'),name=page.getByTestId('demo-delayed-name'),draft=page.getByTestId('demo-delayed-draft'),budget=page.getByTestId('demo-delayed-budget');
        await name.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        assert.equal(await name.inputValue(),'');
        await page.getByTestId('demo-delayed-load').click();
        await page.getByTestId('demo-delayed-status').filter({hasText:'Loading for'}).waitFor();
        await draft.fill('My local segment');
        await page.getByTestId('demo-delayed-status').filter({hasText:'Loaded.'}).waitFor();
        assert.equal(await name.inputValue(),'Returning customers');
        assert.equal(await draft.inputValue(),'My local segment');
        assert.equal(await budget.inputValue(),'$1,234.57');
        await budget.focus();assert.equal(await budget.inputValue(),'1234.5678');
        assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
        await section.screenshot({path:process.env.TEMP+'/neoui-delayed-binding-mobile.png'});
        await page.getByTestId('demo-delayed-restart').click();
        await page.getByTestId('demo-delayed-status').filter({hasText:'Inputs are ready.'}).waitFor();
        await name.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        assert.equal(await name.inputValue(),'');assert.equal(await draft.inputValue(),'');
        // An obsolete load cannot populate a new example after Start again.
        await page.getByTestId('demo-delayed-load').click();
        await page.getByTestId('demo-delayed-status').filter({hasText:'Loading for'}).waitFor();
        await page.getByTestId('demo-delayed-restart').click();
        await page.getByTestId('demo-delayed-status').filter({hasText:'Inputs are ready.'}).waitFor();
        await page.waitForTimeout(1700);assert.equal(await name.inputValue(),'');
        assert.deepEqual(errors,[]);
    }finally{await browser.close();}
});
