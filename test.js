const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const context = await browser.newContext();
  const page = await context.newPage();

  await page.goto('http://localhost:5000/');

  console.log("Current URL:", page.url());

  const h2 = await page.locator('h2').textContent();
  console.log("h2:", h2);

  await browser.close();
})();
