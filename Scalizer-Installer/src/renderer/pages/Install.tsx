import path from 'path';

import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const fs = window.require('fs');

export const copyFileSync = (source: string, target: string) => {
  let targetFile = target;

  // If target is a directory, a new file with the same name will be created
  if (fs.existsSync(target)) {
    if (fs.lstatSync(target).isDirectory()) {
      targetFile = path.join(target, path.basename(source));
    }
  }

  fs.writeFileSync(targetFile, fs.readFileSync(source));
};

export function copyFolderRecursiveSync(source: string, target: string) {
  let files = [];

  // Check if folder needs to be created or integrated
  const targetFolder = path.join(target, path.basename(source));
  if (!fs.existsSync(targetFolder)) {
    fs.mkdirSync(targetFolder);
  }

  // Copy
  if (fs.lstatSync(source).isDirectory()) {
    files = fs.readdirSync(source);
    files.forEach((file: string) => {
      const curSource = path.join(source, file);
      if (fs.lstatSync(curSource).isDirectory()) {
        copyFolderRecursiveSync(curSource, targetFolder);
      } else {
        copyFileSync(curSource, targetFolder);
      }
    });
  }
}

export default function Install() {
  const navigate = useNavigate();

  const routeChange = (value: string) => {
    navigate(value);
  };

  const install = () => {
    copyFolderRecursiveSync(
      `${
        process.env.NODE_ENV === 'production' ? './resources' : '.'
      }/assets/sources/Scalizer-Alpha`,
      'C:\\Program Files\\'
    );
  };

  useEffect(() => {
    install();

    setTimeout(() => {
      routeChange('/');
    }, 1500);
  });

  return (
    <>
      <h1 className="Hello-Title">Installing...</h1>
    </>
  );
}
