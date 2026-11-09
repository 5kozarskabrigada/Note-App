import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { getUserNotes, deleteNote } from '../api/api';
import './NotesList.css';

const NotesList = () => {
    const [notes, setNotes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const fetchNotes = useCallback(async () => {
        try {
            const data = await getUserNotes();
            setNotes(Array.isArray(data) ? data : []);
        } catch (err) {
            if (err.message.includes('Unauthorized')) {
                localStorage.removeItem('authToken');
                navigate('/login');
            }
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, [navigate]);

    useEffect(() => {
        fetchNotes();

        const handleNoteCreated = () => {
            console.log('Note created event received');
            fetchNotes();
        };

        window.addEventListener('noteCreated', handleNoteCreated);

        return () => {
            window.removeEventListener('noteCreated', handleNoteCreated);
        };
    }, [fetchNotes]);

    const formatDate = (dateString) => {
        const options = { year: 'numeric', month: 'short', day: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    };

    const handleDelete = async (noteId, e) => {
        e.stopPropagation();
        try {
            await deleteNote(noteId);
            setNotes(notes.filter(note => note.id !== noteId));
            toast.success('Note deleted successfully');
        } catch (error) {
            toast.error(`Failed to delete note: ${error.message}`);
        }
    };

    if (loading) return <div className="loading">Loading notes...</div>;
    if (error) return <div className="error">Error: {error}</div>;

    return (
        <div className="notes-list-container">
            <h2>Your Notes</h2>
            <div className="notes-grid">
                {notes.length > 0 ? (
                    notes.map(note => (
                        <div
                            key={note.id}
                            className="note-card"
                            onClick={() => navigate(`/note/${note.id}`)}
                        >
                            <h3>{note.title || 'Untitled Note'}</h3>
                            <div className="note-preview">
                                {note.encryptedContent && (
                                    <div dangerouslySetInnerHTML={{
                                        __html: note.encryptedContent.substring(0, 100) + '...'
                                    }} />
                                )}
                            </div>
                            <div className="note-meta">
                                <span>{formatDate(note.createdAt)}</span>
                                <button
                                    className="delete-btn"
                                    onClick={(e) => handleDelete(note.id, e)}
                                >
                                    Delete
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <p className="empty-message">No notes found. Create your first note!</p>
                )}
            </div>
        </div>
    );
};

export default NotesList;