import React, { useState } from 'react';
import { Link, Outlet } from 'react-router-dom';
import './Layout.css'; 

const Layout = () => {
    const [isSidebarMinimized, setIsSidebarMinimized] = useState(false);
   

    const handleSignOut = () => {
       
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('username');
        localStorage.removeItem('userId');

       
        window.location.href = '/login';
    };

    const toggleSidebar = () => {
        setIsSidebarMinimized((prev) => !prev);
    };

    return (
        <div className={`layout ${isSidebarMinimized ? 'sidebar-minimized' : ''}`}>
            <aside className={`sidebar ${isSidebarMinimized ? 'minimized' : ''}`}>
                <div className="sidebar-header">
                    <button className="sidebar-toggle" onClick={toggleSidebar}>
                        {isSidebarMinimized ? '>' : '<'}
                    </button>
                    <br />
                    <br />
                    <br />
                    <h2>Notes</h2>
                </div>
                <nav className="nav-links">
                    <Link to="/home">Dashboard</Link>
                    <Link to="/note/new">Create New Note</Link>
                    <Link to="/notes">Old Notes</Link>
                    {/* <Link to="/profile">Profile</Link> */}
                </nav>
                <div className="bottom-section">
                    <button onClick={handleSignOut}>Logout</button>
                </div>
            </aside>
            <main className="main-content">
                <Outlet /> {}
            </main>
        </div>
    );
};

export default Layout;