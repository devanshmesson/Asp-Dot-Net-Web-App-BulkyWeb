$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#productTable').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "15%" },
            { data: 'isbn', "width": "15%" },
            { data: 'author', "width": "10%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) { 
                    return `<div class="w-75 btn-group">
                    <a href="/admin/product/upsert/${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"</i> Edit </a>
                    <a href="/admin/product/delete/${data}" class="btn btn-danger mx-2"><i class="bi bi-trash-fill"</i> Delete </a>
                    </div>`
                },
                "width": "30%"
            }
        ]
    });
}