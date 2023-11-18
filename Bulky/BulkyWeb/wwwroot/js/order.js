var dataTable;

$(document).ready(function () {

    let url = window.location.search;
    let statuses = ["pending", "inprocess", "completed", "approved", "all"];

    for (let i = 0; i < statuses.length; i++){
    if (url.includes(statuses[i]))
        loadDataTable(statuses[i]);
    }
});

function loadDataTable(status) {
    dataTable = $('#orderTable').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'id', "width": "15%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'applicationUser.email', "width": "10%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                    <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"</i> Edit </a>
                    </div>`
                },
                "width": "30%"
            }
        ]
    });
}
