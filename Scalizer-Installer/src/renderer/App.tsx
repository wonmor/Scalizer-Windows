import { MemoryRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';

import Hello from './pages/Hello';
import Docs from './pages/Docs';
import Install from './pages/Install';
import Uninstall from './pages/Uninstall';

export default function App() {
  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={<Hello />} />
          <Route path="/docs" element={<Docs />} />
          <Route path="/install" element={<Install />} />
          <Route path="/uninstall" element={<Uninstall />} />
        </Routes>
      </Router>
    </>
  );
}
