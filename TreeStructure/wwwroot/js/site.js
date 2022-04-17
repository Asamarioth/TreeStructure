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
         <ul class="list-group">
            <TreeItem :model="this.nodes" v-if="!loading" @addNode="addNodeClick" @removeNode="removeNodeClick" @editNode="editNodeClick" @sort="sortNode"></TreeItem >
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
        Nazwa węzła:
        <input v-model="this.currentBranch.name">
        Gałąź:
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
            if (!this.searchTree(this.nodes, this.currentBranch.id)) {
                alert("Złe id rodzica")
                return
            }
            if (!this.newNodeName || this.newNodeName.length == 0) {
                alert("Nazwa węzła nie może być pusta")
                return
            }

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
        async removeNode() {

            if (!this.searchTree(this.nodes, this.currentBranch.id)) {
                alert("Węzeł nie istnieje")
                this.closeModal()
                return
            }

            const res = await fetch("/Home/DeleteNode?id=" + this.currentBranch.id, {
                method: "delete",
                headers: {
                    "Content-Type": "application/json"
                },
            })
            this.closeModal()
            this.getAllData();
        },
        async editNode() {
            let node = this.searchTree(this.nodes, this.currentBranch.id)
            let parent = this.searchTree(this.nodes, this.currentBranch.parentId)
            if (!node) {
                alert("Węzeł nie istnieje")
                this.closeModal()
                return
            }
            if (!this.currentBranch.name || this.currentBranch.name.trim().length == 0) {
                alert("Nazwa węzła nie może być pusta")
                return
            }
            if (!parent) {
                alert("Rodzic nie istnieje")
                return
            }
            if (this.currentBranch.id == this.currentBranch.parentId) {
                alert("Nie można przenieść węzła do samego siebie")
                return
            }
            if (this.searchTree(node, this.currentBranch.parentId)) {
                alert("Nie można przenieść węzła do jego dziecka")
                return
            }
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
        sortNode(id, direction) {
            this.searchTree(this.nodes, id).children.sort((a, b) => {
                if (a.name < b.name) return -1 * direction;
                if (a.name > b.name) return 1 * direction;
                return 0;
            });
        },
        closeModal() {
            this.showModalAdd = false;
            this.showModalRemove = false;
            this.showModalEdit = false;
            this.currentBranch = {
                id: -1,
                name: "Main",
                parentId: -1
            }
            this.newNodeName = "";

        },
        searchTree(node, id) {
            if (node.id == id) {
                return node;
            } else {
                for (var i = 0; i < node.children.length; i++) {
                    var result = this.searchTree(node.children[i], id);
                    if (result) return result;
                }
            }
        }

    },
    mounted() {
        this.getAllData()
    }


}