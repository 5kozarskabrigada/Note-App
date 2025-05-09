import jwtDecode from 'jwt-decode';

export function isTokenExpired(token) {
    if (!token || typeof token !== 'string' || token.split('.').length !== 3) {
        console.warn('Invalid or malformed token:', token);
        return true;
    }

    try {
        const decoded = jwtDecode(token);
        return decoded.exp < Date.now() / 1000;
    } catch (error) {
        console.error('Failed to decode token:', error);
        return true;
    }
}
