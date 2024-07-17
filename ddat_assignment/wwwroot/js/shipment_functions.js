var $tableRows = $('#shipment-table tbody tr');
var totalRows = $tableRows.length;
var rowsPerPage = 10;
var currentPage = 1;
var totalPages = Math.ceil(totalRows / rowsPerPage);

function loadShipmentData(searchTerm) {
    $.ajax({
        url: '/Driver/LoadShipmentData',
        type: 'GET',
        data: { searchTerm: searchTerm },
        success: function (data) {
            $('#shipmentTable').empty();
            $('#shipmentTable').html(data);
            updateTableData();
        },
        error: function () {
            alert('Error loading shipment data.');
        }
    });
}

function updateTableData() {
    var $tableRows = $('#shipment-table tbody tr');
    var totalRows = $tableRows.length;
    var rowsPerPage = 10;
    var currentPage = 1;
    var totalPages = Math.ceil(totalRows / rowsPerPage);

    $('span.results-count').text(totalRows);

    showPage(currentPage);
    updatePaginationButtons();
}

function showPage(page) {
    var start = (page - 1) * rowsPerPage;
    var end = start + rowsPerPage - 1;
    $tableRows.hide().slice(start, end + 1).show();
    updatePaginationButtons();
}

function updatePaginationButtons() {
    var currentPage = 1; 
    var totalPages = Math.ceil(totalRows / rowsPerPage);

    $('button.prev').prop('disabled', currentPage === 1);
    $('button.next').prop('disabled', currentPage === totalPages || totalRows === 0);
}
