const { chromium } = require('playwright');

(async () => {
    const browser = await chromium.launch();
    const page = await browser.newPage();
    await page.goto('http://localhost:5035/Cuenta/Autenticacion/Login');
    console.log(await page.title());
    console.log(page.url());
    await browser.close();
})();
