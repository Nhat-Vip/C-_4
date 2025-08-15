
    let page = 1;
    const limit = 10;
    const tableBody = document.getElementById('eventTableBody');

    async function loadEvents() {
            try {
                const response = await fetch('/GetEvents?page=' + page + '&limit=' + limit);
    if (!response.ok) throw new Error('Lỗi khi tải dữ liệu');
    const data = await response.json();
    tableBody.innerHTML = '';
                data.events.forEach(ev => {
                    const row = document.createElement('tr');
    row.innerHTML = `
    <td>${ev.eventId}</td>
    <td>${ev.eventName}</td>
    <td>${ev.eventAddress || 'Chưa có'}</td>
    <td>${ev.startEvent}</td>
    <td>${ev.endDateTime}</td>
    <td>${ev.eventType}</td>
    <td>${ev.eventStatus}</td>
    <td>${ev.userName}</td>
    <td>
        <button class="btn btn-success btn-sm me-2" onclick="approveEvent(${ev.eventId})">Duyệt</button>
        <button class="btn btn-secondary btn-sm" onclick="cancelEvent(${ev.eventId})">Hủy</button>
    </td>
    `;
    tableBody.appendChild(row);
                });
                if (data.events.length === 0 && page > 1) page--;
            } catch (error) {
        console.error('Lỗi:', error);
    tableBody.innerHTML = '<tr><td colspan="9">Lỗi khi tải dữ liệu</td></tr>';
            }
        }

    async function approveEvent(eventId) {
            if (confirm('Bạn có chắc chắn muốn duyệt sự kiện này?')) {
                try {
                    const response = await fetch('/UpdateEventStatus', {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify({eventId, status: 'Approved' })
                    });
    const data = await response.json();
    alert(data.message);
    loadEvents();
                } catch (error) {
        console.error('Lỗi:', error);
    alert('Lỗi khi duyệt sự kiện');
                }
            }
        }

    async function cancelEvent(eventId) {
            if (confirm('Bạn có chắc chắn muốn hủy sự kiện này?')) {
                try {
                    const response = await fetch('/UpdateEventStatus', {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify({eventId, status: 'Cancel' })
                    });
    const data = await response.json();
    alert(data.message);
    loadEvents();
                } catch (error) {
        console.error('Lỗi:', error);
    alert('Lỗi khi hủy sự kiện');
                }
            }
        }

    loadEvents();