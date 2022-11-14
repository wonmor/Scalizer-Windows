import { useNavigate } from 'react-router-dom';

import Footer from './Footer';

export default function Hello() {
  const navigate = useNavigate();

  const routeChange = (path: string) => {
    navigate(path);
  };

  return (
    <>
      <h1 className="Hello-Title">Install Scalizer.</h1>

      <p className="Hello-Description">
        Display Scaling on Windows, <b>Reimagined</b>
      </p>

      <div className="Hello">
        <button
          className="Hello-Button"
          type="button"
          onClick={() => routeChange('/docs')}
        >
          <span className="Glyph" role="img" aria-label="books">
            💿
          </span>
          Install
        </button>

        <a
          href="https://www.buymeacoffee.com/wonmor"
          target="_blank"
          rel="noreferrer"
        >
          <button type="button">
            <span className="Glyph" role="img" aria-label="heart">
              ❤️
            </span>
            Donate
          </button>
        </a>
      </div>
      <Footer />
    </>
  );
}
