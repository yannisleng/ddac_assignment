$(document).ready(function () {
    $('#searchTerm').on('keyup', function () {
        var searchTerm = $(this).val().trim();
        loadShipmentData(searchTerm);
    });

    $('#bthAll').on('click', function () {
        loadShipmentData("");
    });

    $('#hs-as-table-table-filter-dropdown').click(function () {
        $('.hs-dropdown-menu').toggleClass('hidden opacity-0');
    });

    $('input[type="checkbox"]').change(function () {
        $('input[type="checkbox"]').not(this).prop('checked', false);
        var filter = $(this).next('span').text().trim();
        loadShipmentData(filter === "All" ? "" : filter);
    });

    showPage(currentPage);
    $('span.results-count').text(totalRows);
    updatePaginationButtons();

    $('button.next').on('click', function () {
        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
        }
    });

    $('button.prev').on('click', function () {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
        }
    });
});