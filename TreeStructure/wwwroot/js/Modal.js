export default {
    props: {
        show:Boolean
    },
    template: `
      <div v-if="show" class="modal-mask" name="modalAdd">
      <div class="modal-wrapper">
        <div class="modal-container">
          <div class="modal-header">
            <slot name="header">default header</slot>
          </div>

          <div class="modal-body">
            <slot name="body">default body</slot>
          </div>

          <div class="modal-footer">
          <slot name="footer">
              <button
                class="modal-default-button"
                @click="$emit('add')"
              >OK</button>
              <button
                class="modal-default-button"
                @click="$emit('close')"
              >Zamknij</button>
          </slot>
          </div>
        </div>
      </div>
    </div>
    `
}