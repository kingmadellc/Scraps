const {webkit}=require('playwright');
const fs=require('fs'),path=require('path');
(async()=>{
 const out=path.resolve('PlaytestCaptures/web-v045');fs.mkdirSync(out,{recursive:true});
 const browser=await webkit.launch({headless:false});
 const context=await browser.newContext({viewport:{width:1366,height:1024},hasTouch:true,isMobile:true,deviceScaleFactor:1});
 const page=await context.newPage(),errors=[],checks=[],states={};
 page.on('pageerror',e=>errors.push(e.message));page.on('console',m=>{if(m.type()==='error')errors.push(m.text());});
 const state=async()=>JSON.parse(await page.evaluate(()=>window.render_game_to_text()));
 const check=(name,ok)=>{checks.push({name,passed:!!ok});if(!ok)throw new Error(name);};
 const shot=async(name)=>{await page.screenshot({path:path.join(out,name+'.png')});states[name]=await state();};
 try{
  await page.goto('http://127.0.0.1:8765/?v=0.4.5');await page.locator('#start').click();
  await page.waitForFunction(()=>typeof window.render_game_to_text==='function',{},{timeout:180000});await page.waitForTimeout(1500);await shot('title');
  check('Latest version loaded',(await state()).version==='0.4.5');
  await page.touchscreen.tap(270,635);await page.waitForTimeout(2500);await shot('avenue');check('Touch New Adventure enters play',(await state()).mode==='playing');
  await page.touchscreen.tap(1127,911);await page.waitForTimeout(350);await shot('discovery');check('Touch Search acquires starting snack',(await state()).bag===1);await page.waitForTimeout(2200);
  await page.keyboard.press('Space');await page.waitForTimeout(110);await page.keyboard.press('t');await page.waitForTimeout(1250);await shot('keyboard-flip');check('Keyboard flip lands and scores',(await state()).trickScore>=150);
  const before=(await state()).trickScore;await page.touchscreen.tap(1280,900);await page.waitForTimeout(100);await page.touchscreen.tap(1280,729);await page.waitForTimeout(1250);await shot('touch-flip');check('Touch Jump then Trick lands and scores',(await state()).trickScore>=before+150);check('Safe flip causes no health damage',(await state()).health===100);
  const start=(await state()).position;await page.keyboard.down('w');await page.waitForTimeout(900);await page.keyboard.up('w');await page.waitForTimeout(250);const moved=(await state()).position;check('Forward movement works',Math.hypot(moved.x-start.x,moved.z-start.z)>1);
  await page.touchscreen.tap(1147,77);await page.waitForTimeout(900);await shot('den');check('Touch Den fast travel reaches basement',(await state()).position.y< -2);
  await page.touchscreen.tap(772,911);await page.waitForTimeout(300);await shot('stash');check('Stash opens paused menu',(await state()).mode==='paused');
  await page.touchscreen.tap(355,471);await page.waitForTimeout(300);check('Unload banks find',(await state()).bag===0&&(await state()).banked===1);
  await page.touchscreen.tap(984,471);await page.waitForTimeout(300);await shot('furnishings');
  await page.touchscreen.tap(266,914);await page.waitForTimeout(200);await page.touchscreen.tap(984,645);await page.waitForTimeout(300);await shot('favorites');
  await page.touchscreen.tap(355,389);await page.waitForTimeout(300);await shot('pinned');
  await page.touchscreen.tap(266,914);await page.waitForTimeout(200);await page.touchscreen.tap(355,850);await page.waitForTimeout(400);check('Den menu returns to movement',(await state()).mode==='playing');
  const saved=await state();await page.reload();await page.locator('#start').click();await page.waitForFunction(()=>typeof window.render_game_to_text==='function',{},{timeout:180000});await page.waitForTimeout(1200);await page.touchscreen.tap(270,635);await page.waitForTimeout(1800);await shot('reloaded');const reloaded=await state();check('Browser reload preserves collection and style',reloaded.banked===saved.banked&&reloaded.trickScore===saved.trickScore&&reloaded.bestLanding===saved.bestLanding);
  check('No browser runtime errors',errors.length===0);
 }catch(e){errors.push(e.stack||e.message);await page.screenshot({path:path.join(out,'failure.png')}).catch(()=>{});process.exitCode=1;}
 finally{fs.writeFileSync(path.join(out,'report.json'),JSON.stringify({checks,errors,states},null,2));console.log(JSON.stringify({checks,errors}));await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
