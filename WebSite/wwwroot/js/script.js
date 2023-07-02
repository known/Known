function showDocNav() {
    var docNav = document.getElementById('docNav');
    docNav.classList.add('show');
}
function hideDocNav() {
    var docNav = document.getElementById('docNav');
    docNav.classList.remove('show');
}
function codeHighlight() {
    hljs.highlightAll();
}