import socket,json,traceback,sys,os

current_path = os.path.dirname(os.path.abspath(__file__))
sys.path.append(f'{current_path}/')


import os
import argparse
import pathlib
import copy
import torch
import sampling
import tokenizers
import rwkv_cpp_model
import rwkv_cpp_shared_library
import json
from typing import Optional
processed_tokens: list[int] = []
logits: Optional[torch.Tensor] = None
state: Optional[torch.Tensor] = None
def chat_code(MODEL_NAME,LANGUAGE = 'English',PROMPT_TYPE = 'QA',):
    global processed_tokens, logits, state
    # Provides terminal-based chat interface for RWKV model.
    # Usage: python chat_with_bot.py C:\rwkv.cpp-169M.bin
    # Prompts and code adapted from https://github.com/BlinkDL/ChatRWKV/blob/9ca4cdba90efaee25cfec21a0bae72cbd48d8acd/chat.py

    

    
    # ======================================== Script settings ========================================

    # English, Chinese, Japanese
    #LANGUAGE: str = 'English'
    # QA: Question and Answer prompt to talk to an AI assistant.
    # Chat: chat prompt (need a large model for adequate quality, 7B+).
    #PROMPT_TYPE: str = 'QA'

    MAX_GENERATION_LENGTH: int = 250

    # Sampling temperature. It could be a good idea to increase temperature when top_p is low.
    TEMPERATURE: float = 0.8
    # For better Q&A accuracy and less diversity, reduce top_p (to 0.5, 0.2, 0.1 etc.)
    TOP_P: float = 0.5
    # Penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
    PRESENCE_PENALTY: float = 0.2
    # Penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
    FREQUENCY_PENALTY: float = 0.2

    END_OF_LINE_TOKEN: int = 187
    END_OF_TEXT_TOKEN: int = 0

    # =================================================================================================






    parser = argparse.ArgumentParser(description='Provide terminal-based chat interface for RWKV model')
    parser.add_argument('model_path', help='Path to RWKV model in ggml format')
    #args = parser.parse_args()

    script_dir: pathlib.Path = pathlib.Path(os.path.abspath(__file__)).parent

    with open(script_dir / 'prompt' / f'{LANGUAGE}-{PROMPT_TYPE}.json', 'r') as json_file:
        prompt_data = json.load(json_file)

        user, bot, separator, init_prompt = prompt_data['user'], prompt_data['bot'], prompt_data['separator'], prompt_data['prompt']

    assert init_prompt != '', 'Prompt must not be empty'

    print('Loading 20B tokenizer')
    tokenizer_path = script_dir / '20B_tokenizer.json'
    tokenizer = tokenizers.Tokenizer.from_file(str(tokenizer_path))

    library = rwkv_cpp_shared_library.load_rwkv_shared_library()
    print(f'System info: {library.rwkv_get_system_info_string()}')

    print('Loading RWKV model')
    model = rwkv_cpp_model.RWKVModel(library, MODEL_NAME)

    prompt_tokens = tokenizer.encode(init_prompt).ids
    prompt_token_count = len(prompt_tokens)

    # =================================================================================================

    

    def process_tokens(_tokens: list[int], new_line_logit_bias: float = 0.0) -> None:
        global processed_tokens, logits, state

        processed_tokens += _tokens

        for _token in _tokens:
            logits, state = model.eval(_token, state, state, logits)

        logits[END_OF_LINE_TOKEN] += new_line_logit_bias

    state_by_thread: dict[str, dict] = {}

    def save_thread_state(_thread: str) -> None:
        state_by_thread[_thread] = {
            'tokens': copy.deepcopy(processed_tokens),
            'logits': copy.deepcopy(logits),
            'state': copy.deepcopy(state)
        }

    def load_thread_state(_thread: str) -> None:
        global processed_tokens, logits, state

        thread_state = state_by_thread[_thread]

        processed_tokens = copy.deepcopy(thread_state['tokens'])
        logits = copy.deepcopy(thread_state['logits'])
        state = copy.deepcopy(thread_state['state'])

    # =================================================================================================

    print(f'Processing {prompt_token_count} prompt tokens, may take a while')

    process_tokens(tokenizer.encode(init_prompt).ids)

    save_thread_state('chat_init')
    save_thread_state('chat')

    print(f'\nChat initialized! Your name is {user}. Write something and press Enter. Use \\n to add line breaks to your message.')

    #自定义修改部分-start
    client_socket.sendall('1'.encode())
    
    while True:
        recv_msg = receive()
        #格式化消息数据
        response_msg = json.loads(recv_msg.decode())
        if response_msg['operation'] == 'send' and model != None:
            if not response_msg['Language']:
                Language = response_msg['Language']
            MAX_GENERATION_LENGTH = response_msg['token_count']
            TEMPERATURE = response_msg['temperature']
            TOP_P = response_msg['top_p']
            FREQUENCY_PENALTY = response_msg['alpha_frequency']
            PRESENCE_PENALTY = response_msg['alpha_presence']
            print(f'{user}{separator}{response_msg["ctx"]}')
        elif response_msg['operation'] == 'status':
            client_socket.sendall('1'.encode())
            continue;
        elif response_msg['operation'] == 'GetName':
            client_socket.sendall(f"{user},{bot}".encode())
            continue;
        elif response_msg['operation'] == 'stop':
            if Language == 'Chinese':
                print("已停止")
            elif Language == 'English':
                print("Stop")
            elif Language == 'Japanes':
                print("止まりました")
            client_socket.sendall('success'.encode())
            return
        # Read user input
        user_input = response_msg["ctx"]
        msg = user_input.replace('\\n', '\n').strip()

        temperature = TEMPERATURE
        top_p = TOP_P

        if '-temp=' in msg:
            temperature = float(msg.split('-temp=')[1].split(' ')[0])

            msg = msg.replace('-temp='+f'{temperature:g}', '')

            if temperature <= 0.2:
                temperature = 0.2

            if temperature >= 5:
                temperature = 5

        if '-top_p=' in msg:
            top_p = float(msg.split('-top_p=')[1].split(' ')[0])

            msg = msg.replace('-top_p='+f'{top_p:g}', '')

            if top_p <= 0:
                top_p = 0

        msg = msg.strip()

        # + reset --> reset chat
        if msg == '+reset':
            load_thread_state('chat_init')
            save_thread_state('chat')
            print(f'{bot}{separator} Chat reset.\n')
            #自定义修改部分-start
            sendMsg('Chat reset.')
            sendMsg('{----end----}')
            #自定义修改部分-end
            continue
        elif msg[:5].lower() == '+gen ' or msg[:3].lower() == '+i ' or msg[:4].lower() == '+qa ' or msg[:4].lower() == '+qq ' or msg.lower() == '+++' or msg.lower() == '++':

            # +gen YOUR PROMPT --> free single-round generation with any prompt. Requires Novel model.
            if msg[:5].lower() == '+gen ':
                new = '\n' + msg[5:].strip()
                state = None
                processed_tokens = []
                process_tokens(tokenizer.encode(new).ids)
                save_thread_state('gen_0')

            # +i YOUR INSTRUCT --> free single-round generation with any instruct. Requires Raven model.
            elif msg[:3].lower() == '+i ':
                new = f'''
Below is an instruction that describes a task. Write a response that appropriately completes the request.

# Instruction:
{msg[3:].strip()}

# Response:
'''
                state = None
                processed_tokens = []
                process_tokens(tokenizer.encode(new).ids)
                save_thread_state('gen_0')

            # +qq YOUR QUESTION --> answer an independent question with more creativity (regardless of context).
            elif msg[:4].lower() == '+qq ':
                new = '\nQ: ' + msg[4:].strip() + '\nA:'
                state = None
                processed_tokens = []
                process_tokens(tokenizer.encode(new).ids)
                save_thread_state('gen_0')

            # +qa YOUR QUESTION --> answer an independent question (regardless of context).
            elif msg[:4].lower() == '+qa ':
                load_thread_state('chat_init')

                real_msg = msg[4:].strip()
                new = f'{user}{separator} {real_msg}\n\n{bot}{separator}'

                process_tokens(tokenizer.encode(new).ids)
                save_thread_state('gen_0')

            # +++ --> continue last free generation (only for +gen / +i)
            elif msg.lower() == '+++':
                try:
                    load_thread_state('gen_1')
                    save_thread_state('gen_0')
                except Exception as e:
                    #自定义修改部分-start
                    sendMsg('{----end----}')
                    #自定义修改部分-end
                    print(e)
                    continue

            # ++ --> retry last free generation (only for +gen / +i)
            elif msg.lower() == '++':
                try:
                    load_thread_state('gen_0')
                except Exception as e:
                    print(e)
                    #自定义修改部分-start
                    sendMsg('{----end----}')
                    #自定义修改部分-end
                    continue
            thread = 'gen_1'

        else:
            # + --> alternate chat reply
            if msg.lower() == '+':
                try:
                    load_thread_state('chat_pre')
                except Exception as e:
                    print(e)
                    #自定义修改部分-start
                    sendMsg('{----end----}')
                    #自定义修改部分-end
                    continue
            # chat with bot
            else:
                load_thread_state('chat')
                new = f'{user}{separator} {msg}\n\n{bot}{separator}'
                process_tokens(tokenizer.encode(new).ids, new_line_logit_bias=-999999999)
                save_thread_state('chat_pre')

            thread = 'chat'

            # Print bot response
            print(f'> {bot}{separator}', end='')

        start_index: int = len(processed_tokens)
        accumulated_tokens: list[int] = []
        token_counts: dict[int, int] = {}

        for i in range(MAX_GENERATION_LENGTH):
            for n in token_counts:
                logits[n] -= PRESENCE_PENALTY + token_counts[n] * FREQUENCY_PENALTY

            token: int = sampling.sample_logits(logits, temperature, top_p)

            if token == END_OF_TEXT_TOKEN:
                print()
                break

            if token not in token_counts:
                token_counts[token] = 1
            else:
                token_counts[token] += 1

            process_tokens([token])

            # Avoid UTF-8 display issues
            accumulated_tokens += [token]

            decoded: str = tokenizer.decode(accumulated_tokens)

            if '\uFFFD' not in decoded:
                #自定义修改部分-start
                sendMsg(decoded)
                #自定义修改部分-end
                print(decoded, end='', flush=True)

                accumulated_tokens = []

            if thread == 'chat':
                if '\n\n' in tokenizer.decode(processed_tokens[start_index:]):
                    break

            if i == MAX_GENERATION_LENGTH - 1:
                print()

        save_thread_state(thread)
        #自定义修改部分-start
        sendMsg('{----end----}')





HOST = 'localhost'  # 服务器主机地址
PORT = 9999         # 服务器端口
client_socket = None
server_socket = None
buffer_size = 1024
def startServer():
    global client_socket,server_socket,Language
    # 创建 socket 对象，指定协议
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    # 绑定IP和端口
    server_socket.bind((HOST,PORT))
    # 监听
    server_socket.listen()
    print("等待客户端连接/Waiting for connections/接続を待ちます")
    client_socket, client_addr = server_socket.accept()
        
    # 接收消息并处理
    print("connected:",client_addr)
    while True:
        recv_msg = receive()
        #格式化消息数据
        msg = json.loads(recv_msg.decode())
        if msg['operation'] == 'start':
            try:
                _modelpath = msg['modelName']
                _PROMPT_TYPE = msg['PROMPT_TYPE']
                _CHAT_LANG = msg['CHAT_LANG']
                Language = msg['Language']
                chat_code(_modelpath,LANGUAGE = _CHAT_LANG,PROMPT_TYPE=_PROMPT_TYPE) 
            except:
                traceback.print_exc()
                sendMsg('{----end----}')
        elif msg['operation'] == 'status':
            client_socket.sendall('0'.encode())
        elif msg['operation'] == 'stop':
            return
def receive():
    #优化socket连接传输数据
    length = client_socket.recv(5)
    client_socket.send(b'ready')    #给客户端返回接收长度成功的消息
    length = int(length.decode())   #取得长度

    recv_size = 0 #记录长度
    recv_msg = b''

    while recv_size < length:
        data = client_socket.recv(buffer_size)
        if not data:
            if Language == 'Chinese':
                print("客户端已关闭连接,将退出进程")
            elif Language == 'English':
                print("The client has closed the connection and will exit the process")
            elif Language == 'Japanes':
                print("クライアントは接続を閉じ,プロセスを終了します")
            client_socket.close()
            break;
        recv_msg += data
        recv_size += len(data)
    return recv_msg

def sendMsg(msg):
    length = len(msg.encode())
    client_socket.send((str(length)).encode())
    responses = client_socket.recv(5)
    if responses.decode() == 'ready':
        client_socket.send(msg.encode())

startServer()


        #自定义修改部分-end
