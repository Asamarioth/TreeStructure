export default {
    name: 'TreeItem',
    props: {
        model:Object
    },
    data() {
        return {
            isOpen:false
        }
    },
    computed: {
        isFolder() {
            return this.model.children
        }
    },
    methods: {
        toggle() {
            if (this.isFolder) {
                this.isOpen = !this.isOpen
            }
        },
        changeType() {
            if (!this.isFolder) {
                this.model.children = []
                this.isOpen = true
            }
        },
        addNodeC(id, parentName){
            this.$emit('addNode', id, parentName)
        },
        removeNodeC(id){
            this.$emit('removeNode', id)
        },
        editNodeC(model){
            this.$emit('editNode', model)
        }
    },
    template: 
        `
        <li>
            <div>
                <span
                @click="toggle"
                @dblclick="changeType">
                {{model.name}}
                </span>
                <button v-if=" model.id !=0" @click="$emit('removeNode',model.id)" class="remove-btn">Usuń</button>
                <button v-if=" model.id !=0" @click="$emit('editNode',model)" class="remove-btn">Edytuj</button>
            </div>
            <ul v-show="isOpen" v-if="isFolder">
                <TreeItem v-for="model in model.children" :model="model" @addNode="addNodeC" @removeNode="removeNodeC" @editNode="editNodeC"> </TreeItem>
            <li @click="$emit('addNode',model.id, model.name)">+</li>
            </ul>
</li>
        `
}