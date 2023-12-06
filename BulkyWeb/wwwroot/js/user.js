var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#userTable').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "10%" },
            { data: '', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                    <a href="/admin/user/upsert/${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"</i> Edit </a>`
                    
                },
                "width": "30%"
            }
        ]
    });
}