// Wait for DOM to load
document.addEventListener('DOMContentLoaded', function() {
    // --- Member Management Setup ---
    const members = [];
    const memberForm = document.getElementById('memberForm');
    const memberModal = document.getElementById('memberModal');
    let bsMemberModal = null;

    if (memberModal) {
        bsMemberModal = new bootstrap.Modal(memberModal);
    }

    if (memberForm) {
        memberForm.addEventListener('submit', function(e) {
            e.preventDefault();

            const newMember = {
                id: members.length + 1,
                firstName: document.getElementById('memberFirstName').value,
                lastName: document.getElementById('memberLastName').value,
                email: document.getElementById('memberEmail').value,
                phone: document.getElementById('memberPhone').value,
                type: document.getElementById('memberType').value,
                status: document.getElementById('memberStatus').value,
                date: new Date().toLocaleDateString()
            };

            members.push(newMember);
            updateMembersTable();
            updateStats();

            if (bsMemberModal) {
                bsMemberModal.hide();
            }
            memberForm.reset();
            showToast('Asociado guardado exitosamente');
        });
    }

    function updateMembersTable() {
        const tbody = document.getElementById('membersTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';

        members.forEach(member => {
            const statusBadge = member.status === 'activo'
                ? '<span class="badge bg-success">Activo</span>'
                : '<span class="badge bg-secondary">Inactivo</span>';

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>#${member.id.toString().padStart(3, '0')}</td>
                <td>${member.firstName} ${member.lastName}</td>
                <td>${member.email}</td>
                <td>${member.phone}</td>
                <td><span class="badge bg-info text-dark">${member.type}</span></td>
                <td>${statusBadge}</td>
                <td>${member.date}</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary me-1" onclick="editMember(${member.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteMember(${member.id})"><i class="bi bi-trash"></i></button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    }

    function updateStats() {
        const elTotal = document.getElementById('totalMembers');
        const elActive = document.getElementById('activeMembers');
        const elInactive = document.getElementById('inactiveMembers');

        if (elTotal) elTotal.textContent = members.length;
        if (elActive) elActive.textContent = members.filter(m => m.status === 'activo').length;
        if (elInactive) elInactive.textContent = members.filter(m => m.status === 'inactivo').length;
    }

    // Initialize with empty state
    updateMembersTable();
    updateStats();

    // Make functions globally available for inline onclick handlers
    window.editMember = function(id) {
        showToast('Función de edición en desarrollo');
    };

    window.deleteMember = function(id) {
        if (confirm('¿Está seguro de eliminar este asociado?')) {
            const index = members.findIndex(m => m.id === id);
            if (index > -1) {
                members.splice(index, 1);
                updateMembersTable();
                updateStats();
                showToast('Asociado eliminado');
            }
        }
    };
});

function showToast(message, type = 'success') {
    const toastContainer = document.getElementById('toastContainer');
    if (!toastContainer) return;

    const toastId = 'toast-' + Date.now();
    const bgClass = type === 'success' ? 'bg-success' : 'bg-danger';

    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });
}
