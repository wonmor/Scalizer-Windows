import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import screenshotOne from '../../../assets/screenshots/Instruction-1.png';
import screenshotTwo from '../../../assets/screenshots/Instruction-2.png';
import screenshotThree from '../../../assets/screenshots/Instruction-3.png';

export default function Docs() {
  const navigate = useNavigate();

  const [buttonCount, setButtonCount] = useState(0);

  const routeChange = (path: string) => {
    navigate(path);
  };

  const getDescription = () => {
    switch (buttonCount) {
      default:
      case 0:
        return (
          <>
            <h1>Check Requirements.</h1>

            <p className="Preview-Description-Text">
              There are a couple of steps you should take before installing our
              software. First of which is setting the display custom scaling.
              This enables our program to take over the default Windows scaling
              settings.
            </p>
          </>
        );
      case 1:
        return (
          <>
            <h1>Step One</h1>

            <p className="Preview-Description-Text">
              First, navigate to{' '}
              <b style={{ color: 'lightblue' }}>Windows Settings</b> and select
              <b style={{ color: 'lightcoral' }}> Display</b>.
            </p>

            <img
              className="Wardrobe-Item-Image"
              src={screenshotOne}
              alt="firstScreenshot"
            />
          </>
        );
      case 2:
        return (
          <>
            <h1>Step Two</h1>

            <p className="Preview-Description-Text">
              Then, click the <b style={{ color: 'lightblue' }}>Scale</b>{' '}
              option.
            </p>

            <img
              className="Wardrobe-Item-Image"
              src={screenshotTwo}
              alt="secondScreenshot"
            />
          </>
        );

      case 3:
        return (
          <>
            <h1>Step Three</h1>

            <p className="Preview-Description-Text">
              Turn on the{' '}
              <b style={{ color: 'lightcoral' }}>Set custom scaling</b> option,
              and the system will prompt you to sign out. Do so.
            </p>

            <img
              className="Wardrobe-Item-Image"
              src={screenshotThree}
              alt="secondScreenshot"
            />
          </>
        );
      case 4:
        return (
          <>
            <h1>Everything OK?</h1>

            <p className="Preview-Description-Text">
              Have you completed all the steps? If so, click the button below to
              continue...
            </p>
          </>
        );
      case 5:
        return (
          <>
            <h1>
              Consider Becoming a <b style={{ color: 'lightcoral' }}>Donor</b>.
            </h1>

            <p className="Preview-Description-Text">
              We are actively recruiting donors to help fund our project. If you
              would like me to continue developing useful tools for our everyday
              technologies, press the button below to be redirected to our
              donation page.
            </p>
          </>
        );
    }
  };

  return (
    <>
      <div className="Preview-Container">
        {getDescription()}
        <div className="Preview-Button-Container">
          <button
            className="Preview-Button-Install"
            type="button"
            onClick={() => {
              setButtonCount(buttonCount + 1);

              if (buttonCount >= 5) {
                routeChange('/install');
              }
            }}
          >
            <span className="Glyph" role="img" aria-label="house">
              {buttonCount === 4 ? <>😉</> : <>👍</>}
            </span>
            {buttonCount === 4 ? <>Ok, Good to Go!</> : <>Next</>}
          </button>
        </div>
      </div>
      {buttonCount !== 5 ? (
        <button type="button" onClick={() => routeChange('/')}>
          <span className="Glyph" role="img" aria-label="house">
            🏠
          </span>
          Home
        </button>
      ) : (
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
      )}
    </>
  );
}
