$(document).ready(function () {

    const sidebarItems = $('.sidebar-item')
    sidebarItems.each(function () {
        $(this).click(function () {
            sidebarItems.removeClass('bg-ok-boss-2 text-ok-boss-9 hover:bg-ok-boss-3 hover:text-ok-boss-7')
            $(this).addClass('bg-ok-boss-2 text-ok-boss-9 hover:bg-ok-boss-3 hover:text-ok-boss-7')
        })
    })
})