import TreeItem from './TreeItem.js'
import Modal from './Modal.js'
export default {
    components: {
        TreeItem,
        Modal
    },
    data() {
        return {
            treeData: null,
            nodes: {
                id: 0,
                name: "Main",
                children: []
            },
            loading: true,
            showModalAdd: false,
            showModalRemove: false,
            showModalEdit: false,
            currentBranch: {
                id: -1,
                name: "Main",
                parentId: -1
            },
            newNodeName: ""
        }
    },
    template: `
         <ul>
            <TreeItem :model="this.nodes" v-if="!loading" @addNode="addNodeClick" @removeNode="removeNodeClick" @editNode="editNodeClick"></TreeItem >
            <h2 v-else>Loading</h2>
        </ul >
        <br />
        
        <Modal :show="showModalAdd" @add="addNode" @close="closeModal">
        <template v-slot:header>
        <h3>Dodaj węzeł</h3>
        </template>
        <template v-slot:body>
        Wybrana gałąź
        <input v-model="this.currentBranch.name" disabled required>
        Nazwa węzła
        <input v-model="this.newNodeName" required>
        </template>
        </Modal>

        <Modal :show="showModalRemove" @add="removeNode" @close="closeModal">
        <template v-slot:header>
        <h3>Usuń węzeł</h3>
        </template>
        <template v-slot:body>
        Czy na pewno chcesz usunąć węzeł?
        </template>
        </Modal>

        <Modal :show="showModalEdit" @add="editNode" @close="closeModal">
        <template v-slot:header>
        <h3>Edytuj węzeł</h3>
        </template>
        <template v-slot:body>
        <input v-model="this.currentBranch.name">
        <select v-model="this.currentBranch.parentId">
            <option value=0>Main</option>
            <option v-for="node in this.treeData" :value="node.id">{{node.name}}</option>
        </select>
        </template>
        </Modal>
        `,
    methods: {
        async getAllData() {
            const res = await fetch("/Home/GetAll");
            this.treeData = await res.json();
            this.nodes.children = this.constructTree(this.treeData)
            this.loading = false

        },
        constructTree(treeData) {
            var keys = [];
            treeData.map(x => {
                x.children = [];
                keys.push(x.id);
            });
            var roots = treeData.filter(x => {
                return x.parentId == 0
            });
            var nodes = [];
            roots.map(x => nodes.push(x));
            while (nodes.length > 0) {
                var node = nodes.pop();
                var children = treeData.filter(x => {
                    return x.parentId == node.id
                });
                children.map(x => {
                    node.children.push(x);
                    nodes.push(x)
                });
            }
            return roots;
        },
        async addNode() {
            const data = {
                ParentId: this.currentBranch.id,
                Name: this.newNodeName
            }
            const res = await fetch("/Home/InsertNode", {
                method: "put",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data),
            })
            this.closeModal()
            this.getAllData();
        },
        async removeNode(){
            console.log(this.currentBranch.id)
            const res = await fetch("/Home/DeleteNode?id="+ this.currentBranch.id, {
                method: "delete",
                headers: {
                    "Content-Type": "application/json"
                },
            })
            this.closeModal()
            this.getAllData();
        },
        async editNode(){
            const data = {
                Id: this.currentBranch.id,
                Name: this.currentBranch.name,
                ParentId: this.currentBranch.parentId
            }
            const res = await fetch("/Home/UpdateNode", {
                method: "patch",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data),
            })
            this.closeModal()
            this.getAllData();
        },
        addNodeClick(id, parentName) {
            this.currentBranch.id = id;
            this.currentBranch.name = parentName;
            this.showModalAdd = true;  
        },
        removeNodeClick(id) {
            this.currentBranch.id = id;
            this.showModalRemove = true;
        },
        editNodeClick(model) {
            this.currentBranch.id = model.id;
            this.currentBranch.name = model.name;
            this.currentBranch.parentId = model.parentId;
            this.showModalEdit = true;
        },
        closeModal(){
            this.showModalAdd = false;
            this.showModalRemove = false;
            this.showModalEdit = false;
            this.currentBranch = {
                id: -1,
                name: "Main",
                parentId: -1
            }
            this.newNodeName = "";

        }

    },
    mounted() {
        this.getAllData()
    }


}