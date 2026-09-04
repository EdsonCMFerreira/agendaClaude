const form = document.querySelector('#agendaForm');
const formPanel = document.querySelector('#formPanel');
const rows = document.querySelector('#agendaRows');
const emptyState = document.querySelector('#emptyState');
const searchInput = document.querySelector('#searchInput');
const itemId = document.querySelector('#itemId');
const descriptionInput = document.querySelector('#descriptionInput');
const dateInput = document.querySelector('#dateInput');
const valueInput = document.querySelector('#valueInput');
const formTitle = document.querySelector('#formTitle');
const formError = document.querySelector('#formError');
const toast = document.querySelector('#toast');
const allItems = [];
let items = [];
let newestFirst = true;

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });
const date = new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' });

document.querySelector('#todayLabel').textContent = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'long' }).format(new Date());

document.querySelector('#newButton').addEventListener('click', () => openForm());
document.querySelector('#closeButton').addEventListener('click', closeForm);
document.querySelector('#cancelButton').addEventListener('click', closeForm);
searchInput.addEventListener('input', render);
document.querySelector('#sortButton').addEventListener('click', () => { newestFirst = !newestFirst; document.querySelector('#sortButton').firstChild.textContent = newestFirst ? 'Mais recentes ' : 'Mais antigos '; render(); });
form.addEventListener('submit', saveItem);

async function loadItems() {
  const response = await fetch('/api/agenda');
  if (!response.ok) throw new Error('Não foi possível carregar a agenda.');
  items = await response.json();
  render();
}

function render() {
  const query = searchInput.value.trim().toLocaleLowerCase();
  const filtered = items.filter(item => item.descricao.toLocaleLowerCase().includes(query)).sort((a, b) => newestFirst ? new Date(b.data) - new Date(a.data) : new Date(a.data) - new Date(b.data));
  rows.innerHTML = filtered.map(item => `<tr><td><strong>${escapeHtml(item.descricao)}</strong></td><td>${date.format(new Date(item.data))}</td><td>${money.format(item.valor)}</td><td><div class="actions"><button class="action" data-edit="${item.id}">Editar</button><button class="action delete" data-delete="${item.id}">Excluir</button></div></td></tr>`).join('');
  emptyState.classList.toggle('visible', filtered.length === 0);
  document.querySelector('#totalCount').textContent = items.length;
  document.querySelector('#totalValue').textContent = money.format(items.reduce((sum, item) => sum + item.valor, 0));
  const next = items.filter(item => new Date(item.data) >= new Date(new Date().setHours(0, 0, 0, 0))).sort((a, b) => new Date(a.data) - new Date(b.data))[0];
  document.querySelector('#nextDate').textContent = next ? date.format(new Date(next.data)) : '--';
  rows.querySelectorAll('[data-edit]').forEach(button => button.addEventListener('click', () => openForm(Number(button.dataset.edit))));
  rows.querySelectorAll('[data-delete]').forEach(button => button.addEventListener('click', () => deleteItem(Number(button.dataset.delete))));
}

function openForm(id) {
  const item = items.find(entry => entry.id === id);
  itemId.value = item?.id ?? '';
  descriptionInput.value = item?.descricao ?? '';
  dateInput.value = item ? item.data.slice(0, 10) : new Date().toISOString().slice(0, 10);
  valueInput.value = item?.valor ?? '';
  formTitle.textContent = item ? 'Editar compromisso' : 'Novo compromisso';
  formError.textContent = '';
  formPanel.classList.add('open');
  descriptionInput.focus();
}

function closeForm() { formPanel.classList.remove('open'); form.reset(); itemId.value = ''; formError.textContent = ''; }

async function saveItem(event) {
  event.preventDefault();
  const id = itemId.value;
  const payload = { descricao: descriptionInput.value.trim(), data: dateInput.value, valor: Number(valueInput.value) };
  const response = await fetch(id ? `/api/agenda/${id}` : '/api/agenda', { method: id ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  if (!response.ok) { formError.textContent = 'Confira os dados informados.'; return; }
  await loadItems();
  closeForm();
  showToast(id ? 'Compromisso atualizado.' : 'Compromisso adicionado.');
}

async function deleteItem(id) {
  const item = items.find(entry => entry.id === id);
  if (!item || !confirm(`Excluir “${item.descricao}”?`)) return;
  const response = await fetch(`/api/agenda/${id}`, { method: 'DELETE' });
  if (response.ok) { await loadItems(); showToast('Compromisso excluído.'); }
}

function escapeHtml(value) { return value.replace(/[&<>'"]/g, character => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', "'": '&#39;', '"': '&quot;' }[character])); }
function showToast(message) { toast.textContent = message; toast.classList.add('show'); setTimeout(() => toast.classList.remove('show'), 2500); }

loadItems().catch(error => { formError.textContent = error.message; });
