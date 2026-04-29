(function () {
    var key = 'known-theme';

    function readTheme() {
        try {
            var stored = localStorage.getItem(key);
            if (stored === 'light' || stored === 'dark') {
                return stored;
            }
        } catch (error) {
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.style.colorScheme = theme;

        try {
            localStorage.setItem(key, theme);
        } catch (error) {
        }
    }

    var initialTheme = readTheme();
    applyTheme(initialTheme);

    window.knownTheme = {
        set: function (theme) {
            applyTheme(theme === 'dark' ? 'dark' : 'light');
        },
        toggle: function () {
            var currentTheme = document.documentElement.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
            applyTheme(currentTheme === 'dark' ? 'light' : 'dark');
        }
    };

    function initNavMenu() {
        var navRoot = document.querySelector('[data-nav-root]');
        if (!navRoot) {
            return;
        }

        var toggle = navRoot.querySelector('[data-nav-toggle]');
        var panels = navRoot.querySelectorAll('[data-nav-panel]');
        if (!toggle || panels.length === 0) {
            return;
        }

        function setOpen(isOpen) {
            navRoot.classList.toggle('is-open', isOpen);
            panels.forEach(function (panel) {
                panel.classList.toggle('is-open', isOpen);
            });
            toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');

            var icon = toggle.querySelector('i');
            if (icon) {
                icon.classList.toggle('fa-bars', !isOpen);
                icon.classList.toggle('fa-xmark', isOpen);
            }
        }

        toggle.addEventListener('click', function (event) {
            event.stopPropagation();
            setOpen(!navRoot.classList.contains('is-open'));
        });

        panels.forEach(function (panel) {
            panel.addEventListener('click', function (event) {
                if (event.target.closest('a')) {
                    setOpen(false);
                }
            });
        });

        document.addEventListener('click', function (event) {
            if (!navRoot.contains(event.target)) {
                setOpen(false);
            }
        });

        window.addEventListener('resize', function () {
            if (window.innerWidth > 920) {
                setOpen(false);
            }
        });
    }

    function initDocToc() {
        var headings = Array.prototype.slice.call(document.querySelectorAll('.known-docs-markdown h2[id], .known-docs-markdown h3[id]'));
        var links = Array.prototype.slice.call(document.querySelectorAll('[data-toc-link]'));
        if (!headings.length || !links.length) {
            return;
        }

        function syncActiveHeading() {
            var currentId = '';
            for (var i = 0; i < headings.length; i++) {
                if (headings[i].getBoundingClientRect().top <= 140) {
                    currentId = headings[i].id;
                }
            }

            if (!currentId) {
                currentId = headings[0].id;
            }

            links.forEach(function (link) {
                var href = link.getAttribute('href') || '';
                link.classList.toggle('active', href === '#' + currentId || href.endsWith('#' + currentId));
            });
        }

        syncActiveHeading();
        window.addEventListener('scroll', syncActiveHeading, { passive: true });
    }

    function initDocSidebar() {
        var activeDocLink = document.querySelector('[data-doc-active="True"], [data-doc-active="true"]');
        if (!activeDocLink) {
            return;
        }

        var details = activeDocLink.closest('details');
        if (details) {
            details.open = true;
        }

        var sidebarCard = activeDocLink.closest('.known-docs-sidebar-card');
        if (sidebarCard) {
            requestAnimationFrame(function () {
                var targetTop = activeDocLink.offsetTop - 120;
                sidebarCard.scrollTop = targetTop > 0 ? targetTop : 0;
            });
        }
    }

    function initSite() {
        initNavMenu();
        initDocToc();
        initDocSidebar();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSite, { once: true });
    } else {
        initSite();
    }

    document.addEventListener('click', function (event) {
        var copyButton = event.target.closest('[data-copy]');
        if (!copyButton) {
            return;
        }

        var text = copyButton.getAttribute('data-copy') || '';
        var original = copyButton.textContent;

        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(text).then(function () {
                copyButton.textContent = '已复制';
                setTimeout(function () { copyButton.textContent = original; }, 1600);
            }).catch(function () {
                copyButton.textContent = '复制失败';
                setTimeout(function () { copyButton.textContent = original; }, 1600);
            });
        }
    });
})();
