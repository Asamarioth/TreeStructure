export default {
    name: 'TreeItem',
    props: {
        model:Object
    },
    data() {
        return {
            isOpen:false,
            sortDirection:1
        }
    },
    computed: {
        isFolder() {
            return this.model.children
        },
        hasChildren() {
            return this.model.children && this.model.children.length > 0
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
        },
        sortC(id,sortDirection) {
            this.$emit('sort', id, sortDirection)
        },
        sort() {
            this.$emit('sort', this.model.id, this.sortDirection)

            this.sortDirection = this.sortDirection * -1
        }
        
    },
    template: 
        `
        <li class="list-group-item">
            <div>
                <span
                @click="toggle"
                @dblclick="changeType"
                class="node-name"
                :class="{'txt-bold': hasChildren}">
                {{model.name}}
                </span>
                <button v-if=" model.id == 0 && hasChildren" @click="$emit('removeNode',model.id)" class="btn btn-danger btn-list">Usuń drzewo</button>
                <button v-if=" model.id !=0" @click="$emit('removeNode',model.id)" class="btn btn-danger btn-list">Usuń</button>
                <button v-if=" model.id !=0" @click="$emit('editNode',model)" class="btn btn-warning btn-list" >Edytuj</button>
                <button v-if="hasChildren" @click="sort" class="btn btn-info btn-list">Sortuj</button>
            </div>
            <ul v-show="isOpen" v-if="isFolder" class="list-group">
                <TreeItem v-for="model in model.children" :model="model" @addNode="addNodeC" @removeNode="removeNodeC" @editNode="editNodeC" @sort="sortC"> </TreeItem>
            <li @click="$emit('addNode',model.id, model.name)" class="add-li list-group-item">+</li>
            </ul>
</li>
        `
}