// Use the supplied game-test client's unchanged action/state/screenshot workflow,
// selecting Metal on this Mac instead of its hard-coded CPU SwiftShader backend.
const path=require('path');
const client=process.env.WEB_GAME_CLIENT||path.join(process.env.HOME,'.codex/skills/develop-web-game/scripts/web_game_playwright_client.js');
const {createRequire}=require('module');
const {chromium}=createRequire(client)('playwright');
const launch=chromium.launch.bind(chromium);
chromium.launch=options=>launch({...options,args:[...(options.args||[]).filter(a=>!a.startsWith('--use-angle=')),'--use-angle=metal']});
import(require('url').pathToFileURL(client).href);
