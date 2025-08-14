let blockCount = 0;

let disableSeat = false;

let listBlockName = [];

function generateSeats() {

    const rows = parseInt(document.getElementById('rows').value);
    const cols = parseInt(document.getElementById('cols').value);

    if (rows < 1 || cols < 1) {
        alert("Số hàng và cột phải lớn hơn 0");
        return;
    }

    const seatContiner = document.getElementById('seat-content');
    const group = document.createElement('div');

    const name = document.getElementById('seat-name').value;
    if (name == "") {
        alert("Vui Long nhap ten");
        return;
    }
    if (listBlockName.find(item => item == name)) {
        alert("Ten da ton tai");
        return;
    }
    console.log(listBlockName);
    listBlockName.unshift(name);
    group.id = name;
    group.className = 'seat-group';
    group.style.setProperty('--cols', cols);
    group.style.setProperty('--rows', rows);
    group.style.setProperty('--bg',"rgb(158, 115, 115)");

    const group_body = document.createElement('div');
    group_body.className = "seat-group-body";

    const overlay = document.createElement('div');
    overlay.className = "overlay";
    overlay.innerText = name;
    overlay.style.setProperty('--bg',"rgb(158, 115, 115)");

    group_body.appendChild(overlay);

    seatContiner.appendChild(group);
    group.appendChild(group_body);


    // const spacingX = 50;
    // const spacingY = 50;

    let rowsController = '';

    for (let r = 0; r < rows; r++) {
        const row = document.createElement('div');
        row.id = "row" + r;
        rowsController += `
                <tr>
                    <td>Row ${r + 1}</td>
                    <td><input type="text" id="name" oninput="updateRowNameSeat('${group.id}','${row.id}',this,this.value)" class="w-25"></td>
                </tr>`
        for (let c = 0; c < cols; c++) {
            const seat = document.createElement('div');
            seat.className = 'seat';
            seat.addEventListener("click", () => {
                if (disableSeat) {
                    seat.id = 'disable';
                    seat.style.backgroundColor = 'gray';
                }
                else {
                    seat.id = '';
                    seat.style.backgroundColor = '#4CAF50';
                }

            })
            seat.innerText = `${c + 1}`;
            seat.style.left = `${(c * 25)}px`;
            seat.style.top = `${(r * 25)}px`;
            row.appendChild(seat);
        }
        group_body.appendChild(row);
        // fitSeatGroup();
    }

    const listBlock = document.getElementById('accordian');
    listBlock.innerHTML += `<div class="card mb-3" id="${name}">
                                    <div class = "card-header">
                                        <a class = "btn w-100 dropdown-toggle" data-bs-toggle="collapse" href=#${blockCount}>
                                            ${name}
                                        </a>
                                    </div>
                                    <div id=${blockCount} class="collapse" data-bs-parent="#accordian">
                                        <div class="card-body">
                                            <div class="mt-3 mb-3">
                                                <label class="form-label" for="name">
                                                <input type="text" id="name" value="${name}" class="form-control">
                                            </div>
                                            <div class="mb-3">
                                                <label for="rotation" class="form-label">Rotation</label>
                                                <input type="range" id="rotation" min="0" max="360" value="0" class="form-range" oninput="updateSeatValue(this,'${name}',this.value)">
                                                <span>0</span>
                                            </div>
                                            <div class="mb-3">
                                                <input type="color" id="color${name}" onchange="ChangeColor('${group.id}',this.value)">
                                            </div>
                                            <div class="overflow-y-scroll" style="height: 300px;">
                                                <table class="table table-borderless table-light">
                                                    <thead>
                                                        <tr>
                                                            <th>Row</th>
                                                            <th>Name</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        ${rowsController}
                                                    </tbody>
                                                    
                                                </table>
                                            </div>
                                            <div class="mt-3">
                                                <button type="button" class="btn btn-success">Save </button>
                                                <button class="btn btn-danger" onclick="DeleteSeatPlan('test')">Delete Seat Plan</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
            `
    blockCount++;


    makeGroupDraggable(group);


}

function fitSeatGroup() {
    const container = document.getElementById('seat-container');
    const group = document.querySelector('.seat-group');

    const groupWidth = group.offsetWidth;
    console.log(groupWidth);
    const groupHeight = group.offsetHeight;

    const maxScale = Math.min(
        container.clientWidth / groupWidth,
        container.clientHeight / groupHeight
    );

    // Scale toàn bộ group
    group.style.transform = `scale(${maxScale})`;

    // Căn giữa trong container
    const offsetX = (container.clientWidth - groupWidth * maxScale) / 2;
    const offsetY = (container.clientHeight - groupHeight * maxScale) / 2;

    group.style.left = `${offsetX}px`;
    group.style.top = `${offsetY}px`;
}

const DeleteSeatPlan = (id) => {
    console.log(id);
    listBlockName = listBlockName.filter(item => item != id);
    document.querySelectorAll(`#${id}`).forEach(item => {
        item.remove();
    })
}

var IsShowSeat = false;

const ShowSeat = (checkbox) => {
    if (checkbox.checked) {
        IsShowSeat = true
        document.querySelectorAll(".overlay").forEach(item => {
            item.style.display = "none";
        })
    }
    else {
        IsShowSeat=false;
        document.querySelectorAll(".overlay").forEach(item => {
            item.style.display = "block";
        })
    }
}

const Save = () => {
    var listDisableSeat = document.querySelectorAll("#disable");
    listDisableSeat.forEach(e => {
        e.remove();
    })
}

// makeGroupDraggable(document.getElementById('seat-group'));

const Activate = () => {
    if (disableSeat) {
        disableSeat = false;
        document.getElementById("EditingSeats").innerHTML = "Active Seat Editing"
    }
    else {
        disableSeat = true
        document.getElementById("EditingSeats").innerHTML = "Disable Seat Editing"
    }
}

function makeGroupDraggable(el) {
    el.onmousedown = function (e) {
        e.preventDefault();
        let offsetX = e.clientX - el.offsetLeft;
        let offsetY = e.clientY - el.offsetTop;

        var container = document.getElementById("seat-content");

        document.onmousemove = function (e) {
            let x = Math.min((Math.round((e.clientX - offsetX) / 10) * 10));
            let y = Math.min((Math.round((e.clientY - offsetY) / 10) * 10));
            el.style.left = `${x}px`;
            el.style.top = `${y}px`;
        };

        document.onmouseup = function () {
            document.onmousemove = null;
            document.onmouseup = null;
        };
    };
}
function updateSeatValue(e, id, value) {
    var seatContainer = document.querySelector("#seat-container");
    var block = seatContainer.querySelector(`#${id}`);
    block.style.transform = `rotate(${value}deg)`;
    e.nextElementSibling.innerHTML = value;

}
function updateRowNameSeat(groupId, id, input, value) {
    let seat_group = document.querySelector(`#${groupId}.seat-group`);
    let checkRowName = seat_group.innerText;
    let row = seat_group.querySelector(`#${id}`);
    if (checkRowName.includes(value) && value != "") {
        input.value = "";
        alert("Ten hang ghe da ton tai");
        return;
    }
    let rowItems = row.querySelectorAll(".seat");
    console.log(rowItems);
    let count = 0;
    rowItems.forEach(item => {
        item.innerText = value + ++count;
    })
}

function ChangeColor(id, value) {
    let seatGroup = document.querySelector(`#${id}.seat-group`);
    let overlay = seatGroup.querySelector(".overlay");
    seatGroup.style.setProperty('--bg',value);
    overlay.style.setProperty('--bg',value);
}

let currentScale = 1; // scale mặc định
const content = document.getElementById("seat-container");
const container = document.getElementById("viewport");

// const listSeatGroup = document.querySelectorAll(".seat-group")

container.addEventListener("wheel", (e) => {
    e.preventDefault();

    if (e.deltaY < 0) {
        // Lăn lên -> zoom in
        currentScale += 0.05;
    } else {
        // Lăn xuống -> zoom out
        currentScale -= 0.05;
    }

    // Giới hạn scale từ 0.5 đến 2
    currentScale = Math.min(Math.max(currentScale, 0.5), 1.5);
    if (currentScale >= 1) {

        document.querySelectorAll(".overlay").forEach(item => {
            item.style.display = "none";
        })
    }
    else if(IsShowSeat = false) {
        document.querySelectorAll(".overlay").forEach(item => {
            item.style.display = "block";
        })
    }
    // content.style.transform = `scale(${currentScale})`;
    updateTransform();
});





// Di chuyen camera
const viewport = document.getElementById("viewport");
const grid = document.getElementById("seat-container");

let isPanning = false;
let startX, startY;
let offsetX = -grid.offsetWidth / 2;
let offsetY = -grid.offsetHeight / 2;

// Chuột phải để pan
viewport.addEventListener("mousedown", (e) => {
    if (e.button === 2) { // Chuột phải
        isPanning = true;
        viewport.style.cursor = "grabbing";
        startX = e.clientX;
        startY = e.clientY;
        e.preventDefault();
    }
});

document.addEventListener("mouseup", () => {
    isPanning = false;
    viewport.style.cursor = "grab";
});

document.addEventListener("mousemove", (e) => {
    if (!isPanning) return;
    const dx = e.clientX - startX;
    const dy = e.clientY - startY;
    offsetX += dx;
    offsetY += dy;
    // grid.style.transform = `translate(${offsetX}px, ${offsetY}px)`;
    updateTransform();
    startX = e.clientX;
    startY = e.clientY;
});

function updateTransform() {
    document.getElementById("seat-content").style.transform = `scale(${currentScale})`;
    grid.style.transform = `translate3d(${Math.round(offsetX)}px, ${Math.round(offsetY)}px, 0) scale(${currentScale})`;
}
// Ngăn menu chuột phải khi pan
viewport.addEventListener("contextmenu", (e) => e.preventDefault());


