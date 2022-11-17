import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { Notification } from 'electron';

import fs from 'fs';
import Footer from './Footer';

const child = require('child_process').execFile;

const executablePath =
  'C:\\Program Files\\Scalizer-Alpha\\net6.0-windows\\Scalizer.exe';

export default function Hello() {
  const navigate = useNavigate();
  const [alreadyInstalled, setAlreadyInstalled] = useState(false);

  const routeChange = (path: string) => {
    navigate(path);
  };

  useEffect(() => {
    fs.stat(
      'C:\\Program Files\\Scalizer-Alpha',
      (err: unknown, stats: fs.Stats) => {
        try {
          if (stats.isDirectory()) {
            setAlreadyInstalled(true);
          }
        } catch (error) {
          setAlreadyInstalled(false);
        }
      }
    );
  }, []);

  return (
    <>
      <h1 className="Hello-Title">Install Scalizer.</h1>

      <p className="Hello-Description">
        Display Scaling on Windows, <b>Reimagined</b>
      </p>

      {alreadyInstalled && (
        <button
          className="Hello-Button-Start"
          type="button"
          onClick={() => {
            // This line executes the Scalizer-Alpha executable... detached = true enables the sub-program to run independently...
            child(executablePath, { detached: false }, (err: string) => {
              if (err) {
                new Notification({
                  title: 'Unexpected Error Occured.',
                  body: err,
                }).show();
              }
            });
          }}
        >
          <span className="Glyph" role="img" aria-label="books">
            😈
          </span>
          Launch Scalizer
        </button>
      )}

      <div className="Hello">
        {!alreadyInstalled ? (
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
        ) : (
          <button
            className="Hello-Button"
            type="button"
            onClick={() => routeChange('/uninstall')}
          >
            <span className="Glyph" role="img" aria-label="books">
              ⬅️
            </span>
            Uninstall
          </button>
        )}

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
