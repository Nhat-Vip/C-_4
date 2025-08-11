const CheckMail = () => {
    console.log("Da Kiem tra");
    const mail = document.getElementById("Email");
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(mail.value)) {
        alert("Email không hợp lệ");
        return false;
    }
    return true;
}

let currentPage = 1;
let allPage = document.querySelectorAll(".page-link");
const next = document.getElementById("next");
const previous = document.getElementById("previous");
const Next = () => {
    currentPage++;
    if (currentPage == 3) {
        next.classList.add("disabled");
        next.disabled = true;
    }
    else {
        previous.classList.remove("disabled");
        previous.disabled = false;
        next.classList.remove("disabled");
        next.disabled = false;
    }
    allPage.forEach(item => {
        item.classList.remove("active");
        document.querySelector(`[data-page ="${currentPage}"]`).classList.add("active");
    })
}
const Previous = () => {
    currentPage--;
    if (currentPage == 1) {
        previous.disabled = true;
        previous.classList.add("disabled");
    }
    else {
        next.disabled = false;
        next.classList.remove("disabled");
        previous.disabled = false;
        previous.classList.remove("disabled");
    }
    allPage.forEach(item => {
        item.classList.remove("active");
        document.querySelector(`[data-page="${currentPage}"]`).classList.add("active");
    })
}

// function loadTickets(status) {
//         // Gọi API bằng fetch
//         fetch(`/Ticket/GetTickets?status=${encodeURIComponent(status)}`)
//             .then(response => {
//                 if (!response.ok) {
//                     throw new Error('Network response was not ok');
//                 }
//                 return response.json();
//             })
//             .then(data => {
//                 const contentArea = document.getElementById('contentArea');
//                 // Xóa class active từ tất cả nút
//                 document.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active'));
//                 // Thêm class active cho nút được chọn
//                 document.querySelector(`.nav-btn[data-status="${status}"]`).classList.add('active');

//                 if (data.Tickets.length === 0) {
//                     contentArea.innerHTML = `
//                         <img src="~/images/no-data.png" alt="No data" class="no-data-image">
//                         <p class="no-data-text">Bạn chưa có vé ${data.Status.toLowerCase()}</p>
//                     `;
//                 } else {
//                     contentArea.innerHTML = `
//                         <div class="ticket-list">
//                             ${data.Tickets.map(ticket => `<div class="ticket-item">${ticket.title}</div>`).join('')}
//                         </div>
//                     `;
//                 }
//             })
//             .catch(error => {
//                 console.error('Error fetching data:', error);
//                 document.getElementById('contentArea').innerHTML = '<p>Có lỗi xảy ra khi tải dữ liệu.</p>';
//             });
//     }

// Gán sự kiện click cho các nút
document.querySelectorAll('.nav-btn').forEach(button => {
    button.addEventListener('click', () => {
        const status = button.getAttribute('data-status');
        loadTickets(status);
    });
});

// Load mặc định cho "Đang xử lý" khi trang tải
window.addEventListener('load', () => loadTickets('Đang xử lý'));