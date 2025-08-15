const CheckMail = () => {
    console.log("Da Kiem tra");
    const mail = document.getElementById("Email");
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(mail.value)) {
        alert("Email không hợp lệ");
        return false;
    }
    return true;
}

const CheckPhoneNumber = (e) =>{
    const regex = "^(0|\+84)(3|5|7|8|9)[0-9]{8}$";
    if(!regex.test(e.value.trim())){
        e.nextElementSibling.innerHtml="Số điện thoại không hợp lệ";
    }
    else{
        e.nextElementSibling.innerHtml = "";

    }
}

let currentPage = 1;
let allPage = document.querySelectorAll(".page-link");
const next = document.getElementById("next");
const previous = document.getElementById("previous");

const PreviewAvt = (avt) =>{
    const preview = document.getElementById("Avatar");
    const file = avt.files[0];
    preview.src = URL.createObjectURL(file);
}

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


// Gán sự kiện click cho các nút
document.getElementById("UserEvent").querySelectorAll('.nav-btn').forEach(button => {
    button.addEventListener('click', () => {
        const status = button.getAttribute('data-status');
        LoadEvents(status);
    });
});

// Mảng lưu trữ tất cả các sự kiện
var Allevent = [];
// Biến lưu trữ trang hiện tại và kích thước trang
let eventIndex = 1;
const pageSize = 4;
// Hàm tải sự kiện theo trạng thái
const LoadEvents = async (status)=>{
    await fetch(`/User/GetEvent?status=${status}`)
    .then(res=>{
        if (!res.ok) {
            throw new Error('Network response was not ok');
        }
        return res.json();
    })
    .then(data=>{
        console.log("API response:", data);
        const contentArea = document.getElementById('EventArea');
        const eventContainer = document.getElementById("UserEvent");
        eventContainer.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active'));
        // Thêm class active cho nút được chọn
        eventContainer.querySelector(`.nav-btn[data-status="${status}"]`).classList.add('active');
        if (data.events.length === 0) {
            contentArea.innerHTML = `      
                <p class="no-data-text">Bạn không có Event nào</p>
            `;
            document.getElementById("EventPagination").innerHTML = "";
        } else {
            Allevent = data.events;
            RenderEvents();
        }
    })
    .catch(error => {
        console.error('Error fetching data:', error);
        document.getElementById('EventArea').innerHTML = '<p>Có lỗi xảy ra khi tải dữ liệu.</p>';
    });
};


const RenderEvents = () => {
    const contentArea = document.getElementById('EventArea');
    const startIndex = (currentPage - 1) * pageSize;
    const pagedEvents = Allevent.slice(startIndex, startIndex + pageSize);

    contentArea.innerHTML = `
        ${pagedEvents.map(ev => `
            <div class="event-card mb-3 rounded-2 w-100 justify-content-between align-items-center d-flex p-3">
                <div class="d-flex justify-content-center">
                    <img src="${ev.image}" width="150px" alt="">
                    <div class="text-start mx-5">
                        <h4 class="fw-bold">${ev.eventName}</h4>
                        <p>${ev.startEvent}</p>
                    </div>
                </div>
                <div>
                    <a href="/Event/Create/${ev.eventId}" class="btn btn-warning ${ev.eventStatus == 2 ? "disabled" :""}"><i class="fa-solid fa-pen-to-square"></i> Sửa</a>
                    <button class="btn btn-danger ${ev.eventStatus == 2 ? "disabled" : ""}" onclick="Delete(${ev.eventId})"><i class="fa-solid fa-trash"></i> Xóa</button>
                </div>
            </div>
        `).join('')}
    `;
    RenderPagination();
};
const RenderPagination = () => {
    const totalPages = Math.ceil(Allevent.length / pageSize);
    let html = ""; 

    if (currentPage > 1) {
        html += `<li class="page-item"><button class="page-link" onclick="ChangePage(${currentPage - 1})" id="previous"><i
                                class="fa-solid fa-chevron-left"></i></button></li>`;
    }

    for (let i = 1; i <= totalPages; i++) {
        html += `<li class="page-item"><a class="page-link ${i==currentPage ? "active":""}" href="#" onclick="ChangePage(${i})">${i}</a></li>`;
    }

    if (currentPage < totalPages) {
        html += `<li class="page-item"><button class="page-link" onclick="ChangePage(${currentPage + 1})" id="next"><i
                                class="fa-solid fa-chevron-right"></i></button></li>`;
    }
    document.getElementById("EventPagination").innerHTML = html;
};
const ChangePage = (page) => {
    currentPage = page;
    RenderEvents();
};

const Delete = async (id) =>{
    const res = await fetch(`/Event/Delete?id=${id}`);
    if(!res.ok){
        throw new Error('Network response was not ok');
    }
    const data = await res.json();
    console.log(data);
    if(data.err != null){
        showErrorAlert(data.err);
    }
    else{
        showSuccessAlert(data.success);
    }
}

const DeleteTicket = async (id) =>{
    const res = await fetch(`/User/DeleteTicket?id=${id}`);
    if (!res.ok) {
        throw new Error('Network response was not ok');
    }
    const data = await res.json();
    console.log(data);
    if (data.err != null) {
        showErrorAlert(data.err);
    }
    else {
        showSuccessAlert(data.success);
    }
}

// Load ticket
let page = 1;
const limit = 3; // Số vé tải mỗi lần
const tableBody = document.getElementById('ticketTableBody');
const loader = document.getElementById('loader');
const ticketArea = document.querySelector('#TicketArea');
let currentStatus = 'All'; // Mặc định trạng thái ban đầu
let ticketData = []; // Biến lưu trữ dữ liệu vé
const loadMoreTickets = async (status = currentStatus) => {
    console.log(currentStatus);
    loader.style.display = 'block';
    try {
        console.log("Page",page);
        console.log("Limit",limit);

        if (page < 1) page = 1;
        if (limit < 1) limit = 3;

        const response = await fetch(`User/GetTickets?page=${page}&limit=${limit}&status=${status}`, {
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();
        console.log(data);
        if (data.tickets && data.tickets.length > 0) {
            ticketData = data.tickets; // Cập nhật dữ liệu vé
            RenderTickets(data);
            loader.style.display = 'none';
            page++;
        } else {
            loader.style.display = 'none';
            // Có thể thêm thông báo khi không còn dữ liệu
            if (tableBody.children.length === 0) {
                tableBody.innerHTML = `<tr><td colspan="6" class="text-center">Bạn không có vé nào</td></tr>`;
            }
        }
    } catch (error) {
        console.error('Lỗi khi tải vé:', error);
        loader.style.display = 'none';
        tableBody.innerHTML = `<tr><td colspan="6" class="text-center">Có lỗi xảy ra khi tải vé</td></tr>`;
    }
};

const observer = new IntersectionObserver((entries) => {
    if (entries[0].isIntersecting) {
        loadMoreTickets(currentStatus); // Truyền trạng thái hiện tại
    }
}, { threshold: 1.0 });

observer.observe(loader);


function RenderTickets() {
    tableBody.innerHTML = `${ticketData.map(tk=>`
        <tr>
            <td>${tk.eventName || 'Chưa có dữ liệu'}</td>
            <td>${tk.date || 'Chưa có dữ liệu'}</td>
            <td>${tk.location || 'Chưa có dữ liệu'}</td>
            <td>${tk.status || 'Chưa có trạng thái'}</td>
            <td>${tk.quantity || 'Chưa có dữ liệu'}</td>
            <td>${tk.total || 'Chưa có dữ liệu'}</td>
            <td>
                <button class="btn btn-primary" onclick='openModal(${JSON.stringify(tk)})'>Chi tiết</button>
                <button class="btn btn-danger" onclick='DeleteTicket(${tk.ticketId})'>Hủy vé</button>
            </td>

        `).join('')}`; // Render dữ liệu vé
   
}

// Modal
function openModal(ticket) {
    document.getElementById('modalEvent').textContent = ticket.eventName || 'Chưa có dữ liệu';
    document.getElementById('modalDate').textContent = ticket.date || 'Chưa có dữ liệu';
    document.getElementById('modalLocation').textContent = ticket.location || 'Chưa có dữ liệu';
    document.getElementById('modalStatus').textContent = ticket.status || 'Chưa có trạng thái';
    document.getElementById('modalQuantity').textContent = ticket.quantity || 'Chưa có dữ liệu';
    document.getElementById('modalTotal').textContent = ticket.total || 'Chưa có dữ liệu';

    // Hiển thị danh sách ticketDetails
    const ticketDetailsList = document.getElementById('modalTicketDetails');
    ticketDetailsList.innerHTML = ''; // Xóa nội dung cũ
    if (ticket.ticketDetails && ticket.ticketDetails.length > 0) {
        ticket.ticketDetails.forEach(detail => {
            const li = document.createElement('li');
            li.textContent = `Ticket Detail ID: ${detail.ticketDetailId}, Ghế: ${detail.seatId}, Thời gian: ${detail.showTimeId}`;
            ticketDetailsList.appendChild(li);
        });
    } else {
        const li = document.createElement('li');
        li.textContent = 'Không có chi tiết vé';
        ticketDetailsList.appendChild(li);
    }

    new bootstrap.Modal(document.getElementById('ticketModal')).show();
}



// Filter tickets by status
document.getElementById('TicketMenu').addEventListener('click', (e) => {
    if (e.target.classList.contains('nav-btn')) {
        const ticketContainer = document.getElementById("UserTicket");
        ticketContainer.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active'));
        e.target.classList.add('active');
        currentStatus = e.target.getAttribute('data-status');

        // Xóa dữ liệu cũ và tải lại
        tableBody.innerHTML = '';
        page = 1;
        loadMoreTickets(currentStatus); // Tải lại với trạng thái mới
    }
});

// Tải lần đầu
loadMoreTickets(currentStatus);


function showErrorAlert(err) {
    Swal.fire({
        icon: 'error',
        title: err,
        showConfirmButton: false,
        timer: 2000,
        toast: true,
        position: 'top'
    });
}
function showSuccessAlert(ms) {
    Swal.fire({
        icon: 'success',
        title: ms,
        showConfirmButton: false,
        timer: 2000,
        toast: true,
        position: 'top'
    });
}