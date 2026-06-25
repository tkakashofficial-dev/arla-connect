import { createApp } from 'vue'
import { createPinia } from 'pinia'
import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'
import '@fontsource-variable/inter/index.css'
import 'primeicons/primeicons.css'
import './style.css'

import App from './App.vue'
import router from './router'

// PrimeVue components used across the app (registered globally for convenience).
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Textarea from 'primevue/textarea'
import Select from 'primevue/select'
import InputNumber from 'primevue/inputnumber'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Card from 'primevue/card'
import Tag from 'primevue/tag'
import Toast from 'primevue/toast'
import Toolbar from 'primevue/toolbar'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
import Paginator from 'primevue/paginator'
import Dialog from 'primevue/dialog'
import Badge from 'primevue/badge'
import Chart from 'primevue/chart'
import Menu from 'primevue/menu'
import Avatar from 'primevue/avatar'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'
import ConfirmDialog from 'primevue/confirmdialog'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(PrimeVue, {
  theme: { preset: Aura, options: { darkModeSelector: '.dark' } },
})
app.use(ToastService)
app.use(ConfirmationService)

app.component('Button', Button)
app.component('InputText', InputText)
app.component('Password', Password)
app.component('Textarea', Textarea)
app.component('Select', Select)
app.component('InputNumber', InputNumber)
app.component('DataTable', DataTable)
app.component('Column', Column)
app.component('Card', Card)
app.component('Tag', Tag)
app.component('Toast', Toast)
app.component('Toolbar', Toolbar)
app.component('Message', Message)
app.component('ProgressSpinner', ProgressSpinner)
app.component('Paginator', Paginator)
app.component('Dialog', Dialog)
app.component('Badge', Badge)
app.component('Chart', Chart)
app.component('Menu', Menu)
app.component('Avatar', Avatar)
app.component('IconField', IconField)
app.component('InputIcon', InputIcon)
app.component('ConfirmDialog', ConfirmDialog)

app.mount('#app')
