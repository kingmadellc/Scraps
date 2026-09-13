const {webkit}=require('playwright');
const fs=require('fs'),path=require('path');
(async()=>{
 const out=path.resolve(process.argv[2]);fs.mkdirSync(out,{recursive:true});
 const browser=await webkit.launch({headless:false});const context=await browser.newContext({viewport:{width:1366,height:1024},deviceScaleFactor:1,hasTouch:true,isMobile:true,userAgent:'Mozilla/5.0 (iPad; CPU OS 26_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.0 Mobile/15E148 Safari/604.1'});
 const page=await context.newPage();const logs=[];page.on('console',m=>{logs.push(m.type()+': '+m.text());if(m.type()==='error')console.log(m.text().slice(0,500));});page.on('pageerror',e=>{logs.push('PAGEERROR: '+e.message);console.log(e.message);});
 await page.goto('http://127.0.0.1:8765/',{waitUntil:'domcontentloaded'});await page.locator('#start').click();
 await page.waitForFunction(()=>!!window.jimothy,{},{timeout:180000});await page.waitForTimeout(3000);await page.screenshot({path:path.join(out,'menu.png')});console.log('WEBKIT_READY');
 let last=-1;
 while(true){await page.waitForTimeout(100);let c;try{c=JSON.parse(fs.readFileSync(path.join(out,'cmd.json'),'utf8'));}catch{continue;}if(c.id<=last)continue;last=c.id;
  try {if(c.action==='tap')await page.touchscreen.tap(c.x,c.y);else if(c.action==='key'){await page.keyboard.down(c.key);await page.waitForTimeout(c.ms||300);await page.keyboard.up(c.key);}else if(c.action==='reload'){await page.reload();await page.locator('#start').click();await page.waitForFunction(()=>!!window.jimothy,{},{timeout:180000});}else if(c.action==='quit'){fs.writeFileSync(path.join(out,'console.json'),JSON.stringify(logs,null,2));await browser.close();console.log('WEBKIT_QUIT');break;}
  await page.waitForTimeout(c.wait||600);await page.screenshot({path:path.join(out,'frame-'+c.id+'.png')});fs.writeFileSync(path.join(out,'result.json'),JSON.stringify({id:c.id,state:await page.evaluate(()=>typeof window.render_game_to_text==='function'?window.render_game_to_text():null),errors:logs.filter(l=>l.startsWith('PAGEERROR')||l.startsWith('error:'))}));console.log('COMMAND '+c.id);
 }catch(e){fs.writeFileSync(path.join(out,'result.json'),JSON.stringify({id:c.id,error:String(e)}));}
 }
})().catch(e=>{console.error(e);process.exit(1)});
