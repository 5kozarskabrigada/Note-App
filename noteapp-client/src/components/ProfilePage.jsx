import React, { useState } from 'react';

const ProfilePage = () => {
    const [email, setEmail] = useState('');
    const [name, setName] = useState('');
    const [bio, setBio] = useState('');

    const handleSave = () => {
        
        console.log('Saving profile...');
    };

    return (
        <div>
            <h2>Profile</h2>
            <form onSubmit={handleSave}>
                <div>
                    <label>Email: </label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Name: </label>
                    <input
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                </div>
                <div>
                    <label>Bio: </label>
                    <textarea
                        value={bio}
                        onChange={(e) => setBio(e.target.value)}
                    ></textarea>
                </div>
                <button type="submit">Save</button>
            </form>
        </div>
    );
};

export default ProfilePage;
