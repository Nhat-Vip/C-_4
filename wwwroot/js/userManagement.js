let page = 1;
const limit = 10;
const tableBody = document.getElementById('userTableBody');

async function loadUsers() {
    try {
        const response = await fetch('/GetUsers?page=' + page + '&limit=' + limit);
        if (!response.ok) throw new Error('Lỗi khi tải dữ liệu');
        const data = await response.json();
        tableBody.innerHTML = '';
        data.users.forEach(user => {
            const row = document.createElement('tr');
            row.innerHTML = `
                            <td>${user.userId}</td>
                            <td>${user.userName}</td>
                            <td>${user.email}</td>
                            <td>${user.phoneNumber || 'Chưa có'}</td>
                            <td>${user.role}</td>
                            <td><img src="${user.avatar}" alt="Avatar" style="width: 50px; height: 50px;"></td>
                            <td>
                                <button class="btn btn-warning btn-sm me-2" onclick="showAddEditModal(${user.userId})">Sửa</button>
                                <button class="btn btn-danger btn-sm" onclick="deleteUser(${user.userId})">Xóa</button>
                            </td>
                        `;
            tableBody.appendChild(row);
        });
        if (data.users.length === 0 && page > 1) page--;
    } catch (error) {
        console.error('Lỗi:', error);
        tableBody.innerHTML = '<tr><td colspan="7">Lỗi khi tải dữ liệu</td></tr>';
    }
}

function showAddEditModal(userId) {
    const modal = new bootstrap.Modal(document.getElementById('userModal'));
    if (userId) {
        fetch('/GetUsers?userId=' + userId)
            .then(response => response.json())
            .then(data => {
                const user = data.users[0];
                document.getElementById('userId').value = user.userId;
                document.getElementById('userName').value = user.userName;
                document.getElementById('email').value = user.email;
                document.getElementById('phoneNumber').value = user.phoneNumber || '';
                document.getElementById('passWord').value = '';
                document.getElementById('role').value = user.role;
                document.getElementById('avatar').value = user.avatar || '';
                document.getElementById('userModalLabel').textContent = 'Sửa người dùng';
            });
    } else {
        document.getElementById('userId').value = '';
        document.getElementById('userName').value = '';
        document.getElementById('email').value = '';
        document.getElementById('phoneNumber').value = '';
        document.getElementById('passWord').value = '';
        document.getElementById('role').value = 'User';
        document.getElementById('avatar').value = '';
        document.getElementById('userModalLabel').textContent = 'Thêm người dùng';
    }
    modal.show();
}

document.getElementById('userForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const user = {
        UserId: +document.getElementById('userId').value || 0,
        UserName: document.getElementById('userName').value,
        Email: document.getElementById('email').value,
        PhoneNumber: document.getElementById('phoneNumber').value,
        PassWord: document.getElementById('passWord').value,
        Role: Role[document.getElementById('role').value],
        Avatar: document.getElementById('avatar').value
    };
    try {
        const response = await fetch('/SaveUser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(user)
        });
        const data = await response.json();
        alert(data.message);
        bootstrap.Modal.getInstance(document.getElementById('userModal')).hide();
        loadUsers();
    } catch (error) {
        console.error('Lỗi:', error);
        alert('Lỗi khi lưu dữ liệu');
    }
});

async function deleteUser(userId) {
    if (confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
        try {
            const response = await fetch('/DeleteUser/' + userId, { method: 'DELETE' });
            const data = await response.json();
            alert(data.message);
            loadUsers();
        } catch (error) {
            console.error('Lỗi:', error);
            alert('Lỗi khi xóa người dùng');
        }
    }
}

loadUsers();