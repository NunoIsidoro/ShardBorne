import os
import shutil

def copiar_scripts_unity():
    # --- CONFIGURAÇÃO ---
    # Diretoria onde procurar os scripts (ponto . significa a pasta onde o script está)
    origem_raiz = "." 
    
    # Nome da pasta de destino
    nome_pasta_destino = "ALLScripts"
    
    # Pastas específicas onde procurar
    autores_alvo = ["Nuno", "Gabriel", "Marco"]
    
    # Extensões dos ficheiros a copiar (Tupla com os formatos desejados)
    extensoes_alvo = (".cs", ".json")
    # --------------------

    # Detetar o caminho do Ambiente de Trabalho (Desktop) do utilizador atual
    caminho_desktop = os.path.join(os.path.expanduser("~"), "Desktop")
    
    # Definir o caminho final: Desktop/ALLScripts
    caminho_destino = os.path.join(caminho_desktop, nome_pasta_destino)

    print(f"A preparar para copiar ficheiros {extensoes_alvo} para: {caminho_destino}")

    # Criar a pasta ALLScripts no Desktop se não existir
    if not os.path.exists(caminho_destino):
        try:
            os.makedirs(caminho_destino)
            print(f"Pasta '{nome_pasta_destino}' criada com sucesso no Ambiente de Trabalho.")
        except PermissionError:
            print("ERRO: Não tem permissão para criar pastas no Ambiente de Trabalho.")
            return
    else:
        print(f"A pasta '{nome_pasta_destino}' já existe no Desktop. Novos ficheiros serão adicionados.")

    contador_copias = 0

    # Percorrer todas as pastas e subpastas do projeto
    for root, dirs, files in os.walk(origem_raiz):
        
        # Verificar se estamos dentro de uma das pastas dos autores (Nuno, Gabriel, Marco)
        partes_caminho = root.split(os.sep)
        
        # Se nenhuma das pastas alvo estiver no caminho atual, passamos à frente
        if not any(autor in partes_caminho for autor in autores_alvo):
            continue

        for arquivo in files:
            # ALTERAÇÃO AQUI: Verifica se termina com .cs OU .json (usando .lower() para garantir que apanha .CS ou .JSON)
            if arquivo.lower().endswith(extensoes_alvo):
                caminho_origem_arquivo = os.path.join(root, arquivo)
                caminho_final_arquivo = os.path.join(caminho_destino, arquivo)

                # Lógica para evitar sobrescrever ficheiros com o mesmo nome
                base, ext = os.path.splitext(arquivo)
                contador = 1
                while os.path.exists(caminho_final_arquivo):
                    novo_nome = f"{base}_{contador}{ext}"
                    caminho_final_arquivo = os.path.join(caminho_destino, novo_nome)
                    contador += 1

                try:
                    shutil.copy2(caminho_origem_arquivo, caminho_final_arquivo)
                    print(f"[Copiado] {arquivo}")
                    contador_copias += 1
                except Exception as e:
                    print(f"[Erro] Falha ao copiar {arquivo}: {e}")

    print("---")
    print(f"Processo concluído! {contador_copias} ficheiros copiados.")
    print(f"A pasta encontra-se em: {caminho_destino}")

if __name__ == "__main__":
    copiar_scripts_unity()