// ==================== BASE DE DATOS INDEXEDDB ====================
const DB_NAME = 'ASOPROPESMAR_DB';
const DB_VERSION = 1;
let db;

// Inicializar Base de Datos
function initDB() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, DB_VERSION);

        request.onerror = () => reject(request.error);
        request.onsuccess = () => {
            db = request.result;
            showToast('Base de datos conectada', 'success');
            resolve(db);
        };

        request.onupgradeneeded = (event) => {
            const db = event.target.result;

            if (!db.objectStoreNames.contains('members')) {
                const store = db.createObjectStore('members', { keyPath: 'id', autoIncrement: true });
                store.createIndex('email', 'email', { unique: true });
            }

            // Datos de ejemplo
            const tx = event.target.transaction;
            const store = tx.objectStore('members');

            const sampleData = [
                { firstName: 'Walter', lastName: 'González Klegan', email: 'walter@asopropesmar.org', phone: '0981452439', type: 'pescador', status: 'activo', date: '2026-01-01' },
                { firstName: 'Juan', lastName: 'Pérez Mendoza', email: 'juan@email.com', phone: '0987654321', type: 'pescador', status: 'activo', date: '2026-01-05' }
            ];

            sampleData.forEach(m => store.add(m));
        };
    });
}

// CRUD - Obtener todos
function getAllMembers() {
    return new Promise((resolve, reject) => {
        if (!db) return resolve([]);
        const tx = db.transaction('members', 'readonly');
        const store = tx.objectStore('members');
        const request = store.getAll();
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

// Mostrar toast
function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const colors = { success: 'bg-success', danger: 'bg-danger', info: 'bg-info', warning: 'bg-warning' };
    const toast = document.createElement('div');
    toast.className = `toast show ${colors[type]} text-white`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `<div class="toast-body"><i class="bi bi-${type === 'success' ? 'check-circle' : 'info-circle'} me-2"></i>${message}</div>`;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}

// Inicializar
document.addEventListener('DOMContentLoaded', async () => {
    try {
        await initDB();
    } catch(e) {
        console.error("Error init DB: ", e);
    }
});

// Smooth scroll
document.querySelectorAll('a[href^="#"]').forEach(a => {
    a.addEventListener('click', e => {
        const targetAttr = a.getAttribute('href');
        if (targetAttr && targetAttr.startsWith('#')) {
            const targetEl = document.querySelector(targetAttr);
            if(targetEl) {
                e.preventDefault();
                targetEl.scrollIntoView({ behavior: 'smooth' });
            }
        }
    });
});
