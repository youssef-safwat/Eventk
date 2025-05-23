/*!
 * Dashboard scripts - jQuery implementation
 */

$(document).ready(function() {
    // Check if sidebar state exists in localStorage
    if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
        $('body').addClass('sb-sidenav-toggled');
    }
    
    // Toggle the side navigation
    $('#sidebarToggle').on('click', function(e) {
        e.preventDefault();
        $('body').toggleClass('sb-sidenav-toggled');
        // Save sidebar state to localStorage
        localStorage.setItem('sb|sidebar-toggle', $('body').hasClass('sb-sidenav-toggled'));
    });
});
