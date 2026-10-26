const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const context = await browser.newContext();
  const page = await context.newPage();

  await page.goto('http://localhost:5000/Cuenta/Autenticacion/Login');

  await page.waitForTimeout(2000);

  console.log("Current URL:", page.url());

  await browser.close();
})();
