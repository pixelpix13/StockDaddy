const contactEmail = import.meta.env.VITE_CONTACT_EMAIL || 'sales@stockdaddy.app';
const formspreeEndpoint = import.meta.env.VITE_FORMSPREE_ENDPOINT?.trim() || '';

document.getElementById('year').textContent = String(new Date().getFullYear());

const emailLink = document.getElementById('contact-email-link');
if (emailLink) {
  emailLink.href = `mailto:${contactEmail}`;
  emailLink.textContent = contactEmail;
}

const navToggle = document.querySelector('.nav-toggle');
const nav = document.querySelector('.nav');
if (navToggle && nav) {
  navToggle.addEventListener('click', () => {
    const open = nav.classList.toggle('is-open');
    navToggle.setAttribute('aria-expanded', String(open));
  });

  nav.querySelectorAll('a').forEach((link) => {
    link.addEventListener('click', () => {
      nav.classList.remove('is-open');
      navToggle.setAttribute('aria-expanded', 'false');
    });
  });
}

const form = document.getElementById('contact-form');
const statusEl = document.getElementById('form-status');

function setStatus(message, type = '') {
  statusEl.textContent = message;
  statusEl.className = `form-status${type ? ` ${type}` : ''}`;
}

form?.addEventListener('submit', async (event) => {
  event.preventDefault();
  setStatus('');

  const formData = new FormData(form);
  const payload = Object.fromEntries(formData.entries());

  if (!payload.name || !payload.business || !payload.country || !payload.stores || !payload.message) {
    setStatus('Please fill in all required fields.', 'error');
    return;
  }

  const submitBtn = form.querySelector('button[type="submit"]');
  submitBtn.disabled = true;
  setStatus('Sending…');

  try {
    if (formspreeEndpoint) {
      const response = await fetch(formspreeEndpoint, {
        method: 'POST',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          name: payload.name,
          business: payload.business,
          country: payload.country,
          stores: payload.stores,
          phone: payload.phone || '',
          message: payload.message,
          _subject: `StockDaddy inquiry from ${payload.business}`,
        }),
      });

      if (!response.ok) {
        const data = await response.json().catch(() => ({}));
        throw new Error(data.error || 'Unable to send message. Try again or email us directly.');
      }

      form.reset();
      setStatus('Thanks! We will respond within 1 business day.', 'success');
    } else {
      const subject = encodeURIComponent(`StockDaddy inquiry — ${payload.business}`);
      const body = encodeURIComponent(
        [
          `Name: ${payload.name}`,
          `Business: ${payload.business}`,
          `Country: ${payload.country}`,
          `Stores: ${payload.stores}`,
          `Phone: ${payload.phone || '—'}`,
          '',
          payload.message,
        ].join('\n')
      );
      window.location.href = `mailto:${contactEmail}?subject=${subject}&body=${body}`;
      setStatus('Your email app should open. If not, write to us directly.', 'success');
    }
  } catch (err) {
    setStatus(err.message || 'Something went wrong. Email us directly.', 'error');
  } finally {
    submitBtn.disabled = false;
  }
});
