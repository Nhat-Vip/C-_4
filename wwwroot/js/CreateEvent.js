let TabControl = ['[data-bs-target="#EventInfo"]', '[data-bs-target="#EventSeatingChart"]', '[data-bs-target="#EventTicket"]', '[data-bs-target="#EventPayment"]']
var tabIndex = 0;
let ticketGroup = [];
let showTimeIndex = 1;
let allowSaveST = true;
let maxGroupTicket;
let modalInstance;

const SetTabIndex = (index)=>{
    tabIndex = index;
}

const Next = async () => {
    
    if(tabIndex == 0){
       await SaveEventInfo();
    }
    else if(tabIndex == 1){
       await SaveSeatingChart();
    }
    else if(tabIndex == 2){
       await SaveShowTime();
    }
    else if(tabIndex == 3){
        await SavePayment();
        return;
    }
    console.log(tabIndex);
    const tabTrigger = document.querySelector(`${TabControl[tabIndex]}`);
    tabTrigger.classList.remove("disabled");
    const tab = new bootstrap.Tab(tabTrigger);
    tab.show();
}

const openAddModal = (id) => {
    const container = document.querySelector(`[href="#${id}"]`);
    const parent = container.closest(".card");

    console.log(parent.querySelector("#showTimeStart").value);

    if (parent.querySelector("#showTimeStart").value == "" 
    || parent.querySelector("#showTimeEnd").value == "")
    {
        showErrorAlert("Vui lòng nhập ngày bắt đầu và kết thúc sự kiện");
        return;
    }

    const modalElement = document.getElementById('CreateTicket');
    modalElement.querySelector("#ticketId").value = "";
    modalInstance = new bootstrap.Modal(modalElement);
    document.getElementById("IdShowTime").value = id;
    modalInstance.show();

}

const updateTicket = (e) =>{
    const modalElement = document.getElementById('CreateTicket');
    const modal = new bootstrap.Modal(modalElement);

    const container = e.closest(".collapse");
    const parent = e.closest(".ticket");

    document.getElementById("ticketName").value = parent.querySelector("#TicketName").value;
    document.getElementById("Price").value = parent.querySelector("#TicketPrice").value;
    document.getElementById("SeatGroupId").value = parent.querySelector("#SeatGroup").value;
    document.getElementById("MaxTicket").value = parent.querySelector("#MaxTicket").value;
    document.getElementById("TicketSaleStart").value = parent.querySelector("#StartTime").value;
    document.getElementById("TicketSaleEnd").value = parent.querySelector("#EndTime").value;
    document.querySelector("#ticketId").value = `#${container.id} #${parent.id}`;

    modalInstance.show();
}   

const saveTicket = (e) => {
    e.preventDefault();

    const ticketId = document.getElementById("ticketId").value;
    if(ticketId.trim() != "" && ticketId != null){
        console.log("ticketId: ", ticketId);
        const ticket = document.querySelector(ticketId);
        ticket.children[0].innerHTML = `<i class="fa-solid fa-ticket"></i>${document.getElementById("ticketName").value}`;
        ticket.querySelector("#TicketName").value = document.getElementById("ticketName").value;
        ticket.querySelector("#TicketPrice").value = document.getElementById("Price").value;
        ticket.querySelector("#SeatGroup").value = document.getElementById("SeatGroupId").value;
        ticket.querySelector("#MaxTicket").value = document.getElementById("MaxTicket").value;
        ticket.querySelector("#StartTime").value = document.getElementById("TicketSaleStart").value;
        ticket.querySelector("#EndTime").value = document.getElementById("TicketSaleEnd").value;
        modalInstance.hide();
        return;
    }

    const id = document.getElementById("IdShowTime").value;
    const tkName = document.getElementById("ticketName").value;
    const price = document.getElementById("Price").value;
    const seatGruop = document.getElementById("SeatGroupId").value;
    const maxTicket = document.getElementById("MaxTicket").value;
    const startTime = document.getElementById("TicketSaleStart").value;
    const endTime = document.getElementById("TicketSaleEnd").value;


    const ShowTime = document.querySelector(`[href="#${id}"]`);

    const parent = ShowTime.closest(".card");

    const maxStartTime = parent.querySelector("#showTimeEnd").value;
    if(startTime >= maxStartTime){
        document.getElementById("TicketSaleStart").nextElementSibling.innerHTML = "Thời gian bán vé phải nhỏ hơn thời gian kết thúc sự kiện"
        return;
    }
    if(endTime >= maxStartTime){
        document.getElementById("TicketSaleEnd").nextElementSibling.innerHTML = "Thời gian dừng bán vé phải nhỏ hơn thời gian kết thúc sự kiện"
        return;
    }
    else{
        document.getElementById("TicketSaleStart").nextElementSibling.innerHTML = ""
        document.getElementById("TicketSaleEnd").nextElementSibling.innerHTML = ""

    }
    
    const checkSGr = parent.querySelectorAll(".ticket #SeatGroup");
    // console.log(checkSGr);
    // console.log(seatGruop);
    const hasExits = Array.from(checkSGr).find(gr => gr.value == seatGruop);
    if(hasExits != null){
        console.log(hasExits);
        showErrorAlert("1 Nhóm ghế chỉ được 1 loại vé");
        return;
    }
    const container = document.querySelector(`#${id} #TicketGroupContainer`);
    // console.log(container);
    const ticketHtml = `
                <div class="d-flex mb-3 ticket justify-content-between align-items-center p-2 rounded-3" id="ticket${seatGruop}" style="box-shadow: 0 0 5px black;">
                    <p class="text-black fw-bold m-0"><i class="fa-solid fa-ticket"></i>${tkName}</p>
                    <div>
                        <button class="btn btn-warning" onclick="updateTicket(this)">Sửa</button>
                        <button class="btn btn-danger" onclick="deleteTicket(this)">Xóa</button>
                    </div>
                        <input type="hidden" id="TicketName" value="${tkName}" />
                        <input type="hidden" id="TicketPrice" value="${price}" />
                        <input type="hidden" id="SeatGroup" value="${seatGruop}" />
                        <input type="hidden" id="MaxTicket" value="${maxTicket}" />
                        <input type="hidden" id="StartTime" value="${startTime}" />
                        <input type="hidden" id="EndTime" value="${endTime}" />
                </div>
            `

    container.insertAdjacentHTML("beforeend", ticketHtml);

    ShowTime.innerHTML = formatDateTime(parent.querySelector("#showTimeStart").value);
    ShowTime.innerHTML += `<p class="text-black p-0 m-0">${parent.querySelectorAll(".ticket").length} Loại vé</p>`;

    // const modalElement = document.getElementById('CreateTicket');
    // const modal = new bootstrap.Modal(modalElement);
    modalInstance.hide();
    allowSaveST = true;
}

const deleteTicket = (e) => {
    const ticket = e.closest(".ticket");
    ticket.remove();
}
const AddShowTime = () => {
    const container = document.getElementById("ShowTime");
    allowSaveST = false;
    showTimeIndex++;
    const showTimeHtml =
        `
            <div class="card mb-3">
                <div class="card-header d-flex justify-content-between">
                    <a href="#ShowTime${showTimeIndex}" class="btn w-75 text-start" data-bs-toggle="collapse" >Vui lòng nhập thông tin xuất chiếu</a>
                    <button type="button" class=" btn btn-danger" style="height: max-content;" onclick="DeleteShowTime(this)">Xóa</button>
                </div>
                <div class="collapse" id="ShowTime${showTimeIndex}" data-bs-parent="#ShowTime">
                    <div class="card-body">
                        <div class="d-flex">
                            <div class="mb-3 w-100 me-3 mt-3">
                                <label for="showTimeStart" class="form-label" >Thời gian bắt đầu</label>
                                <input type="datetime-local" id="showTimeStart" onchange="ShowTimeStart(this,'ShowTime${showTimeIndex}')" class="form-control">
                            </div>
                            <div class="mb-5 w-100 mt-3">
                                <label for="showTimeEnd" class="form-label">Thời gian kết thúc</label>
                                <input type="datetime-local" id="showTimeEnd" onchange="ChangeEndTime(this)" class="form-control">
                                <span class="text-danger"></span>
                            </div>
                        </div>
                        <div class="mb-3" id="TicketGroupContainer">
                        </div>
                        <div class="mb-3">
                            <button class="btn-showTime fw-bold" onclick="openAddModal('ShowTime${showTimeIndex}')">Thêm loại vé</button>
                        </div>
                    </div>
                </div>
            </div>
        `

    container.insertAdjacentHTML("beforeend", showTimeHtml);

    // showTime.push({ ShowTime: index, ticketGroup: 0 });
}

var imgUrl = "";

const PreviewImg = (input) => {
    const preview = document.getElementById("preview");
    const file = input.files[0];
    if (file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            imgUrl = file;
            preview.style.display = "block";
        }

        reader.readAsDataURL(file);
    }
}
const ShowTimeStart = (e, id) => {
    let title = document.querySelector(`[href="#${id}"]`);
    title.innerHTML = formatDateTime(e.value);
    const parent = e.closest(".card-body");
    const allTickets = parent.querySelectorAll(".ticket");
    console.log(allTickets);
    if(allTickets.length == 0){
        title.innerHTML +=`<p class="text-danger p-0 m-0">Vui lòng thêm ít nhất 1 loại vé</p>`
    }
}

const DeleteShowTime = (e) => {
    const parent = e.closest(".card");
    parent.remove();
}

const ChangeEndTime = (e)=>{
    const parent = e.closest(".d-flex");
    // e.value = formatDateTime(e.value);
    if(e.value <= parent.querySelector("#showTimeStart").value){
        e.nextElementSibling.innerHTML = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu"
        allowSaveST = false;
        return;
    }
    else{
        e.nextElementSibling.innerHTML = "";
        allowSaveST = true;
    }
}

function formatDateTime(date) {
    const d = new Date(date);

    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0
    const year = d.getFullYear();

    const hours = String(d.getHours()).padStart(2, '0');
    const minutes = String(d.getMinutes()).padStart(2, '0');

    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

function CheckNumber(el) {
    const regex = /^\d+(\.\d*)?$/;
    if (!regex.test(el.value)) {
        el.value = el.value.slice(0, -1); // Xóa ký tự cuối nếu không hợp lệ
    }
}

document.getElementById("EventInfo").addEventListener("submit", async function (e) {
    e.preventDefault(); // chỉ chặn sau khi validation OK
    await SaveEventInfo();
});

const validateEventInfo = () =>{
    let hasError = false;

    const form = document.getElementById("EventInfo");
    form.querySelectorAll("span.text-danger").forEach(span => span.textContent = "");

    form.querySelectorAll("[required]").forEach(input => {
        if (!input.value.trim()) {
            const errorSpan = input.parentElement.querySelector("span.text-danger");
            if (errorSpan) {
                errorSpan.textContent = "Vui lòng nhập thông tin này";
            }
            hasError = true;
        }
    });
    const startEvent = form.querySelector("#StartEvent");
    const endEvent = form.querySelector("#EndEvent");

    const startd = new Date(startEvent.value);
    const endd = new Date(endEvent.value);
    const startHour = parseInt(String(startd.getHours()).padStart(2, '0'));
    const endHour = parseInt(String(endd.getHours()).padStart(2, '0'));
    const startDay = parseInt(String(startd.getDate()).padStart(2, '0'));
    const endDay = parseInt(String(endd.getDate()).padStart(2, '0'));

    if(startEvent.value >= endEvent.value && startEvent.value != "" || startHour+2 >= endHour){
        startEvent.nextElementSibling.innerHTML = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc ít nhất 2 tiếng";
        hasError = true;
    }
    else if(new Date(startEvent.value).getTime() < Date.now()){
        startEvent.nextElementSibling.innerHTML = "Thời gian bắt đầu không được thấp hơn thời gian hiện tại";
        hasError = true;
    }
    // else if(startEvent.value == "" || endEvent.value == ""){
    //     startEvent.nextElementSibling.innerHTML = "Vui lòng nhập thông tin này";
    //     endEvent.nextElementSibling.innerHTML = "Vui lòng nhập thông tin này";
    // }
    if(endHour < startHour+2 && endDay <= startDay){
        console.log("EndDay: ",endDay);
        console.log("StartDay: ",startDay);
        endEvent.nextElementSibling.innerHTML = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu ít nhất 2 tiếng";
        hasError = true;

    }
    
    return hasError;

}

document.getElementById("EventInfo").addEventListener("submit",(e)=>{
    e.preventDefault();
})

const SaveEventInfo = async () => {
    console.log("dang luu");
    const fileInput = document.getElementById("EventImage");
    const file = fileInput.files[0];
    const hasError = validateEventInfo();

    if(hasError){
        return;
    }

    const formData = new FormData();
    console.log("EventId: ", document.getElementById("eventId").value);
    formData.append("EventId",document.getElementById("eventId").value ?? null);
    formData.append("EventName", document.getElementById("EventName").value);
    formData.append("ImageFile",file);
    formData.append("Description", document.getElementById("Description").value);
    formData.append("StartDateTime", document.getElementById("StartEvent").value);
    formData.append("EndDateTime", document.getElementById("EndEvent").value);
    formData.append("EventType",document.getElementById("EventType").value);
    formData.append("EventAddress", document.getElementById("EventAddress").value);
    formData.append("UserId", parseInt(document.getElementById("UserId").value));
    
    const res = await fetch("/Event/SaveEventInfo",{
        method: "POST",
        // headers: {"Content-Type":"application/json"},
        body: formData
    });

    if (!res.ok) {
        const text = await res.text(); // Đọc phản hồi dạng text
        console.error("Server error:", text);
        alert("Lỗi từ server: " + text);
        return;
    }

    const data = await res.json();
    console.log("Status:", res.status);
    console.log("Response:", data);

    console.log("EventId",data.eventId);
    console.log(tabIndex);
    tabIndex++;
    console.log(tabIndex);
    document.getElementById("eventId").value = data.eventId;
    console.log("Da luu");
}

const SaveSeatingChart = async () =>{
    const seatContainer = document.getElementById("seat-content");
    const seatGroups = seatContainer.querySelectorAll(".seat-group");
    const eventId = document.getElementById("eventId").value;
    // const seatingChartName = document.getElementById("SeatingChartName").value;
    // if(seatingChartName.trim() == ""){
    //     showErrorAlert("Vui Lòng nhập tên")
    // }
    console.log("EventId",eventId);
    const SeatingCharts = {
        // Name:document.getElementById("SeatingChartName").value,
        EventId: parseInt(eventId),
        SeatGroups:[]
    };
    if(seatGroups.length == 0){
        showErrorAlert("Vui lòng nhập ít nhất 1 nhóm ghế");
        return;
    }
    seatGroups.forEach(sgr=>{
        const seatGroupData = {
            Name:sgr.id,
            Rotate: document.querySelector(`#accordian #${sgr.id}`).querySelector("#rotation").value,
            Color:getComputedStyle(sgr).getPropertyValue('--bg'),
            Cols:parseInt(getComputedStyle(sgr).getPropertyValue('--cols')),
            Rows:parseInt(getComputedStyle(sgr).getPropertyValue('--rows')),
            PosX: parseInt(sgr.style.left)||0,
            PosY: parseInt(sgr.style.top)||0,
            Seats:[]
        }
        // const group = SeatingCharts.SeatingChart.SeatGroup.find(g=>g.Name == sgr.id);
        sgr.querySelectorAll(".seat").forEach(s=>{
            const seatData={
                SeatName: s.innerHTML, 
                PosX: parseInt(s.style.left)||0,
                PosY: parseInt(s.style.top)||0
            }
            seatGroupData.Seats.push(seatData);
        })
        SeatingCharts.SeatGroups.push(seatGroupData);
    })
    console.log("SeatingChart: ",SeatingCharts);
    const res = await fetch(`/Event/SaveSeatingChart?eventId=${eventId}`,{
        method:"POST",
        headers:{"Content-Type":"application/json"},
        body: JSON.stringify(SeatingCharts)
    });

    if (!res.ok) {
        const text = await res.text(); // Đọc phản hồi dạng text
        console.error("Server error:", text);
        alert("Lỗi từ server: " + text);
        return;
    }
    const html = await res.text();

    document.getElementById("dropdown-seatGroup").innerHTML = html;
    maxGroupTicket = document.getElementById("SeatGroupId").options.length;
    console.log("MaxGroupTicket: ",maxGroupTicket);
    console.log("Luu seating chart thanh cong");
    tabIndex++;
}

const SaveShowTime = async () =>{
    let allowSave = true;
    const container = document.getElementById("ShowTime");
    const AllShowTime = container.querySelectorAll(".card");
    // let showTimeCurrent = 1;
    const eventId = document.getElementById("eventId").value;
    const listShowTime = [];
    if(AllShowTime.length == 0){
        showErrorAlert("Vui lòng nhập ít nhất 1 xuất chiếu");
        return;
    }
    AllShowTime.forEach(st=>{

        if (st.querySelector("#showTimeStart").value == ""
            || st.querySelector("#showTimeEnd").value == "") {
            showErrorAlert("Vui lòng nhập ngày bắt đầu và kết thúc sự kiện");
            allowSave = false;
            return;
        }
        if(st.querySelectorAll(".ticket").length < maxGroupTicket){
            showErrorAlert(`Vui Lòng nhập ${maxGroupTicket} loại vé cho nhóm ghế của từng xuất chiếu`);
            allowSave = false;
            return;
        }
        const showTimeData = {
            EventId: parseInt(eventId),
            StartTime: st.querySelector("#showTimeStart").value,
            EndTime: st.querySelector("#showTimeEnd").value,
            ShowTimeTicketGroups:[]
        };
        st.querySelectorAll(".ticket").forEach(tk=>{
            const ticketData = {
                Price: parseFloat(tk.querySelector("#TicketPrice").value.replace(",", ".")),
                TicketSaleStart:tk.querySelector("#StartTime").value,
                TicketSaleEnd:tk.querySelector("#EndTime").value,
                SeatGroupId: tk.querySelector("#SeatGroup").value,
                Name: tk.querySelector("#TicketName").value,
                MaxTicket: tk.querySelector("#MaxTicket").value
                
            }
            console.log("TicketData: ",ticketData);
            showTimeData.ShowTimeTicketGroups.push(ticketData);
        })
        listShowTime.push(showTimeData);
    });
    if(!allowSave){
        // tabIndex--;
        return;
    }
    console.log("ShowTime: ",listShowTime);
    const res = await fetch(`/Event/SaveShowTime`,{
        method:"POST",
        headers:{"Content-Type":"application/json"},
        body: JSON.stringify(listShowTime)
    });

    if (!res.ok) {
        const text = await res.text(); // Đọc phản hồi dạng text
        console.error("Server error:", text);
        alert("Lỗi từ server: " + text);
        return;
    }
    tabIndex++

}
const SavePayment = async () =>{
    const BankNumber = document.getElementById("BankNumber").value;
    const BankName = document.getElementById("BankName").value;
    const EventId = parseInt(document.getElementById("eventId").value);

    const formData = new FormData();
    formData.append("BankName",BankName);
    formData.append("BankNumber",BankNumber);
    formData.append("EventId",EventId);

    const res = await fetch(`/Event/SavePayment`,{
        method:"POST",
        body: formData
    })
    if (!res.ok) {
        const text = await res.text(); // Đọc phản hồi dạng text
        console.error("Server error:", text);
        alert("Lỗi từ server: " + text);
        return;
    }
    const result = await res.json();
    if (result.message == "Success"){
        await showSuccessAlert("Them Event Thanh Cong Cho Duyet");
        setTimeout(()=>{
            window.location.href="/Home/Index";
        },3000);
    }
}
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