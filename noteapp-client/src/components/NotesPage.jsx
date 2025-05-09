import React, { useEffect, useState } from 'react';
import { getNotes } from '../api/api';
import { Link } from 'react-router-dom';

const NotesPage = () => {
    const [notes, setNotes] = useState([]);

    useEffect(() => {
        getNotes().then(setNotes);
    }, []);

    return (
        <div className="notes-grid">
            {notes.map(note => (
                <Link to={`/note/${note.id}`} className="note-card" key={note.id}>
                    <h3>{note.title || 'Untitled Note'}</h3>
                    <div dangerouslySetInnerHTML={{ __html: note.content.slice(0, 100) }} />
                </Link>
            ))}
        </div>
    );
};

export default NotesPage;