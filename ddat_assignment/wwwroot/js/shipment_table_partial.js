var isSubmitting = false;

$(document).ready(function () {
    $('.openModalButton').on('click', function () {
        var shipmentId = $(this).data('shipment-id');
        var shipmentStatus = $(this).data('shipment-status');
        var hasPod = $(this).data('has-pod');

        var states = ["Selangor", "Sabah", "Sarawak", "Peris", "Penang", "Perak", "Johor", "Kelantan", "Kuantan", "Kuala Lumpur", "Kedah", "Melaka", "Negeri Sembilan", "Pahang", "Terengganu"];

        $('#modalTitle').text(shipmentStatus === 'Delivered' ? 'Proof of Delivery' : 'Update Shipment');

        if (shipmentStatus === 'Delivered' && hasPod) {
            $.ajax({
                url: '/Driver/GetProofOfDelivery',
                type: 'GET',
                data: { shipmentId: shipmentId },
                dataType: 'json',
                success: function (data) {
                    var img = $('<img>').attr('src', 'data:' + data.contentType + ';base64,' + data.image).addClass('max-w-full h-auto');
                    var dateDisplay = $('<p class="text-sm font-light mt-5">').text('Delivered on ' + new Date(data.deliveryDate).toLocaleString());
                    $('#modalBody').empty().append(img).append(dateDisplay);
                    $('#saveChangesButton').hide();
                },
                error: function () {
                    $('#modalBody').html('<p>No proof of image provided.</p>');
                    $('#saveChangesButton').hide();
                }
            });
        } else {
            $('#modalBody').html(`
                    <form id="shipmentUpdateForm" enctype="multipart/form-data">
                        <input type="hidden" id="shipmentId" name="shipmentId" value="${shipmentId}" />
                        <label class="block text-sm font-medium mb-2 dark:text-white">Shipment Status</label>
                        <select id="shipmentStatus" name="newStatus" class="py-3 px-4 block w-full border-gray-300 rounded-lg text-sm focus:border-primary focus:ring-primary dark:bg-neutral-800 dark:border-neutral-700 dark:text-neutral-400" aria-required="true">
                            <option value="In Transit">In Transit</option>
                            ${shipmentStatus !== "Pending" ? '<option value="Delivered">Delivered</option>' : ''}
                        </select>

                        <div id="transitLocationContainer" class="mt-5">
                            <label for="transitLocation" class="block text-sm font-medium mb-2 dark:text-white">Warehouse</label>
                            <select id="transitLocation" name="transitLocation" class="py-3 px-4 block w-full border-gray-200 rounded-lg text-sm focus:border-primary focus:ring-primary dark:bg-neutral-800 dark:border-neutral-700 dark:text-neutral-400" aria-required="true">
                                ${states.map(state => `<option value="${state}">${state}</option>`).join('')}
                            </select>
                            <span class="text-sm text-red-600 mt-2"></span>
                        </div>

                        <div id="proofOfDeliveryContainer" class="hidden mt-5">
                            <label for="file-input" class="block text-sm font-medium mb-2 dark:text-white">Proof of Delivery</label>
                            <input type="file" accept="image/*" name="proofOfDelivery" id="file-input" class="block w-full border border-gray-200 shadow-sm rounded-lg text-sm focus:z-10 focus:border-blue-500 focus:ring-blue-500 disabled:opacity-50 disabled:pointer-events-none dark:bg-neutral-900 dark:border-neutral-700 dark:text-neutral-400 file:bg-gray-50 file:border-0 file:me-4 file:py-3 file:px-4 dark:file:bg-neutral-700 dark:file:text-neutral-400">
                        </div>

                        <div class="flex justify-end items-center gap-x-2 py-3 px-4 border-t dark:border-neutral-700 mt-7">
                            <button type="submit" class="py-2 px-3 inline-flex items-center gap-x-2 text-sm font-semibold rounded-lg border border-transparent bg-ok-boss-6 text-white disabled:opacity-50 disabled:pointer-events-none" id="saveChangesButton">
                                Save changes
                            </button>
                        </div>
                    </form>
                `);
            $('#saveChangesButton').show();
        }

        $('#hs-focus-management-modal').removeClass('hidden').addClass('flex');
        $('#modalContent').removeClass('mt-0 opacity-0').addClass('mt-7 opacity-100');

        $('#shipmentStatus').on('change', function () {
            if (this.value === 'In Transit') {
                $('#transitLocationContainer').removeClass('hidden').addClass('block');
                $('#proofOfDeliveryContainer').removeClass('block').addClass('hidden');
            } else if (this.value === 'Delivered') {
                $('#transitLocationContainer').removeClass('block').addClass('hidden');
                $('#proofOfDeliveryContainer').removeClass('hidden').addClass('block');
            }
        });

        $('#shipmentStatus').trigger('change');

        isSubmitting = false;
    });

    $('#closeModalButton').on('click', function () {
        $('#modalContent').removeClass('mt-7 opacity-100').addClass('mt-0 opacity-0');
        setTimeout(function () {
            $('#hs-focus-management-modal').removeClass('flex').addClass('hidden');
        }, 500);

        isSubmitting = false;
    });

    $(document).on('submit', '#shipmentUpdateForm', function (e) {
        e.preventDefault();

        if (isSubmitting) return;

        isSubmitting = true;
        var formData = new FormData(this);
        var shipmentId = $('#shipmentId').val();

        $.ajax({
            url: '/Driver/UpdateShipmentStatus',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                loadShipmentData(shipmentId);
                closeModal();
            },
            error: function (xhr, status, error) {
                alert('Error updating shipment status: ' + xhr.responseText);
            },
            complete: function () {
                isSubmitting = false; 
            }
        });
    });
});

function closeModal () {
    $('#modalContent').removeClass('mt-7 opacity-100').addClass('mt-0 opacity-0');
    setTimeout(function () {
        $('#hs-focus-management-modal').removeClass('flex').addClass('hidden');
    }, 500);

    isSubmitting = false;
}