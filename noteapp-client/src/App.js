import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import LoginPage from './components/LoginPage';
import RegisterPage from './components/RegisterPage';
import HomePage from './components/HomePage';
import NoteEditor from './components/NoteEditor';
import Layout from './components/Layout';
import NotesList from './components/NotesList';
import './App.css';
import 'react-toastify/dist/ReactToastify.css';
import { jwtDecode } from 'jwt-decode';

const AppWrapper = () => {
  return (
    <Router>
      <App />
    </Router>
  );
};

const App = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(() => {
    const token = localStorage.getItem('authToken');
    return token ? !isTokenExpired(token) : false;
  });

  function isTokenExpired(token) {
    try {
      const decoded = jwtDecode(token);
      return decoded.exp < Date.now() / 1000;
    } catch {
      return true;
    }
  }

  useEffect(() => {
    const checkAuth = () => {
      const token = localStorage.getItem('authToken');
      setIsAuthenticated(token ? !isTokenExpired(token) : false);
    };

    const handleStorageChange = () => {
      checkAuth();
    };

    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  return (
    <Routes>
      <Route path="/login" element={!isAuthenticated ? <LoginPage /> : <Navigate to="/home" />} />
      <Route path="/register" element={!isAuthenticated ? <RegisterPage /> : <Navigate to="/home" />} />

      <Route element={<Layout />}>
        <Route
          path="/home"
          element={isAuthenticated ? <HomePage /> : <Navigate to="/login" />}
        />
        <Route
          path="/note/new"
          element={isAuthenticated ? <NoteEditor /> : <Navigate to="/login" />}
        />
        <Route
          path="/note/:id"
          element={isAuthenticated ? <NoteEditor /> : <Navigate to="/login" />}
        />
        <Route
          path="/notes"
          element={isAuthenticated ? <NotesList /> : <Navigate to="/login" />}
        />
        <Route
          path="/"
          element={<Navigate to={isAuthenticated ? "/home" : "/login"} />}
        />
      </Route>
    </Routes>
  );
};

export default AppWrapper;