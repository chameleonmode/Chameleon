import { ChameleonTemplatePage } from './app.po';

describe('Chameleon App', function() {
  let page: ChameleonTemplatePage;

  beforeEach(() => {
    page = new ChameleonTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
