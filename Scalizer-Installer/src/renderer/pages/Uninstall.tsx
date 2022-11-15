import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const fs = window.require('fs');

export default function Uninstall() {
  const navigate = useNavigate();

  const routeChange = (value: string) => {
    navigate(value);
  };

  useEffect(() => {
    fs.rmdirSync('C:\\Program Files\\Scalizer-Alpha', { recursive: true });

    setTimeout(() => {
      routeChange('/');
    }, 1500);
  });

  return <h1 className="Hello-Title">Uninstalling...</h1>;
}
