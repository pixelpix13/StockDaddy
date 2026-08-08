(function () {
  var key = 'stockdaddy-theme';
  var stored = localStorage.getItem(key);
  var preference = stored === 'light' || stored === 'dark' || stored === 'system' ? stored : 'system';
  var dark =
    preference === 'dark' ||
    (preference === 'system' && window.matchMedia('(prefers-color-scheme: dark)').matches);
  var root = document.documentElement;
  if (dark) root.classList.add('dark');
  else root.classList.remove('dark');
  root.style.colorScheme = dark ? 'dark' : 'light';
})();
