// ==================== BASE DE DATOS INDEXEDDB ====================
const DB_NAME = 'ASOPROPESMAR_DB';
const DB_VERSION = 1;
let db;
let currentUser = null;

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
                { firstName: 'Juan', lastName: 'Pérez Mendoza', email: 'juan@email.com', phone: '0987654321', type: 'pescador', status: 'activo', date: '2026-01-05' },
                { firstName: 'María', lastName: 'García López', email: 'maria@email.com', phone: '0976543210', type: 'acuicultor', status: 'activo', date: '2026-01-10' },
                { firstName: 'Carlos', lastName: 'Rodríguez Santos', email: 'carlos@email.com', phone: '0965432109', type: 'procesador', status: 'activo', date: '2026-01-15' },
                { firstName: 'Ana', lastName: 'Martínez Castro', email: 'ana@email.com', phone: '0954321098', type: 'comerciante', status: 'inactivo', date: '2025-12-20' }
            ];

            sampleData.forEach(m => store.add(m));
        };
    });
}

// CRUD - Agregar miembro
function addMember(data) {
    return new Promise((resolve, reject) => {
        const tx = db.transaction('members', 'readwrite');
        const store = tx.objectStore('members');
        const member = { ...data, date: new Date().toISOString().split('T')[0] };
        const request = store.add(member);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

// CRUD - Obtener todos
function getAllMembers() {
    return new Promise((resolve, reject) => {
        const tx = db.transaction('members', 'readonly');
        const store = tx.objectStore('members');
        const request = store.getAll();
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

// CRUD - Eliminar
function deleteMember(id) {
    return new Promise((resolve, reject) => {
        const tx = db.transaction('members', 'readwrite');
        const store = tx.objectStore('members');
        const request = store.delete(id);
        request.onsuccess = () => resolve();
        request.onerror = () => reject(request.error);
    });
}

// Renderizar tabla
async function renderTable() {
    const members = await getAllMembers();
    const tbody = document.getElementById('membersTableBody');
    tbody.innerHTML = '';

    members.forEach(m => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td><strong>ASO-2026-${String(m.id).padStart(4, '0')}</strong></td>
            <td>${m.firstName} ${m.lastName}</td>
            <td>${m.email}</td>
            <td>${m.phone}</td>
            <td><span class="text-capitalize">${m.type}</span></td>
            <td><span class="badge ${m.status === 'activo' ? 'badge-activo' : 'badge-inactivo'}">${m.status}</span></td>
            <td>${m.date}</td>
            <td>
                <button class="btn btn-sm btn-outline-danger" onclick="removeMember(${m.id})">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });

    // Actualizar stats
    document.getElementById('totalMembers').textContent = members.length;
    document.getElementById('activeMembers').textContent = members.filter(m => m.status === 'activo').length;
    document.getElementById('inactiveMembers').textContent = members.filter(m => m.status === 'inactivo').length;
}

// Eliminar miembro
window.removeMember = async (id) => {
    if (confirm('¿Eliminar este asociado?')) {
        await deleteMember(id);
        showToast('Asociado eliminado', 'success');
        renderTable();
    }
};

// Mostrar toast
function showToast(message, type = 'info') {
    const colors = { success: 'bg-success', danger: 'bg-danger', info: 'bg-info', warning: 'bg-warning' };
    const toast = document.createElement('div');
    toast.className = `toast show ${colors[type]} text-white`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `<div class="toast-body"><i class="bi bi-${type === 'success' ? 'check-circle' : 'info-circle'} me-2"></i>${message}</div>`;
    document.getElementById('toastContainer').appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}

// Login (Migrado a OIDC, conservamos try/catch en caso de modal dinámico)
const loginForm = document.getElementById('loginForm');
if (loginForm) {
    loginForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const u = document.getElementById('username').value;
        const p = document.getElementById('password').value;

        if ((u === 'admin' && p === 'admin123') || (u === 'asopropesmar' && p === '123456')) {
            currentUser = { name: u };
            bootstrap.Modal.getInstance(document.getElementById('loginModal')).hide();
            showToast('Bienvenido a ASOPROPESMAR 2026', 'success');
        } else {
            showToast('Credenciales incorrectas', 'danger');
        }
    });
}

// Nuevo miembro
const memberForm = document.getElementById('memberForm');
if (memberForm) {
    memberForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const data = {
        firstName: document.getElementById('memberFirstName').value,
        lastName: document.getElementById('memberLastName').value,
        email: document.getElementById('memberEmail').value,
        phone: document.getElementById('memberPhone').value,
        type: document.getElementById('memberType').value,
        status: document.getElementById('memberStatus').value
    };

    try {
        await addMember(data);
        bootstrap.Modal.getInstance(document.getElementById('memberModal')).hide();
        e.target.reset();
        showToast('Nuevo asociado registrado', 'success');
        renderTable();
    } catch (err) {
        showToast('Error al registrar', 'danger');
    }
    });
}

// Inicializar
document.addEventListener('DOMContentLoaded', async () => {
    await initDB();
    await renderTable();
});

// Smooth scroll
document.querySelectorAll('a[href^="#"]').forEach(a => {
    a.addEventListener('click', e => {
        e.preventDefault();
        document.querySelector(a.getAttribute('href')).scrollIntoView({ behavior: 'smooth' });
    });
});
