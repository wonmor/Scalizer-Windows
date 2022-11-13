import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import screenshotOne from '../../../assets/screenshots/Instruction-1.png';
import screenshotTwo from '../../../assets/screenshots/Instruction-2.png';
import screenshotThree from '../../../assets/screenshots/Instruction-3.png';

export default function Select() {
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
          <p className="Preview-Description-Text">
            There are a couple of steps you should take before installing our
            software. First of which is setting the display custom scaling. This
            enables our program to take over the default Windows scaling
            settings.
          </p>
        );
      case 1:
        return (
          <>
            <p className="Preview-Description-Text">
              First, navigate to Windows Settings and select Display.
            </p>

            <img
              className="Wardrobe-Item-Image"
              src={screenshotOne}
              alt="firstScreenshot"
            />
          </>
        );
      case 2:
        return 'The last step is to restart your computer. This will enable our software to take over the default Windows scaling settings.';
    }
  };

  useEffect(() => {
    if (buttonCount === 3) {
      routeChange('/preview');
    }
  });

  return (
    <>
      <div className="Preview-Container">
        <h1>Check Requirements.</h1>
        {getDescription()}
        <div className="Preview-Button-Container">
          <button
            className="Preview-Button-Install"
            type="button"
            onClick={() => {
              setButtonCount(buttonCount + 1);
            }}
          >
            <span className="Glyph" role="img" aria-label="house">
              {buttonCount === 3 ? <>😉</> : <>👍</>}
            </span>
            {buttonCount === 3 ? <>Ok, Good to Go!</> : <>Next</>}
          </button>
        </div>
      </div>

      <button type="button" onClick={() => setButtonCount(0)}>
        <span className="Glyph" role="img" aria-label="house">
          🏠
        </span>
        Home
      </button>
    </>
  );
}
