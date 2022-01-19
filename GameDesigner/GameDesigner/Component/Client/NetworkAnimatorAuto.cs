﻿#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络同步动画控制器, 自动检测动画被切换后同步给其他客户端
    /// </summary>
    public class NetworkAnimatorAuto : NetworkAnimatorBase
    {
        private void Awake()
        {
            animator = GetComponent<Animator>();
            parameters = new AnimatorParameter[animator.parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = new AnimatorParameter()
                {
                    type = animator.parameters[i].type,
                    name = animator.parameters[i].name,
                    defaultBool = animator.parameters[i].defaultBool,
                    defaultFloat = animator.parameters[i].defaultFloat,
                    defaultInt = animator.parameters[i].defaultInt
                };
            }
            nt.animators.Add(this);
            id = nt.animators.Count - 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (nt.mode == SyncMode.Synchronized)
                return;
            if (Time.time < sendTime)
                return;
            sendTime = Time.time + (1f / rate);
            var nameHash1 = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            for (int i = 0; i < parameters.Length; i++)
            {
                switch (parameters[i].type)
                {
                    case AnimatorControllerParameterType.Bool:
                        var bvalue = animator.GetBool(parameters[i].name);
                        if (parameters[i].defaultBool != bvalue)
                        {
                            parameters[i].defaultBool = bvalue;
                            ClientManager.AddOperation(new Operation(Command.AnimatorParameter, nt.identity)
                            {
                                cmd1 = (byte)id,
                                cmd2 = 1,
                                index1 = i,
                                index2 = bvalue ? 1 : 0
                            });
                        }
                        break;
                    case AnimatorControllerParameterType.Float:
                        var fvalue = animator.GetFloat(parameters[i].name);
                        if (parameters[i].defaultFloat != fvalue)
                        {
                            parameters[i].defaultFloat = fvalue;
                            ClientManager.AddOperation(new Operation(Command.AnimatorParameter, nt.identity)
                            {
                                cmd1 = (byte)id,
                                cmd2 = 2,
                                index1 = i,
                                direction = new Net.Vector3(fvalue, 0, 0)
                            });
                        }
                        break;
                    case AnimatorControllerParameterType.Int:
                        var ivalue = animator.GetInteger(parameters[i].name);
                        if (parameters[i].defaultInt != ivalue)
                        {
                            parameters[i].defaultInt = ivalue;
                            ClientManager.AddOperation(new Operation(Command.AnimatorParameter, nt.identity)
                            {
                                cmd1 = (byte)id,
                                cmd2 = 3,
                                index1 = i,
                                index2 = ivalue
                            });
                        }
                        break;
                }
            }
            if (nameHash != nameHash1)
            {
                nameHash = nameHash1;
                ClientManager.AddOperation(new Operation(Command.Animator, nt.identity)
                {
                    index1 = id,
                    index2 = nameHash1
                });
            }
        }

        public override void OnNetworkPlay(int stateNameHash, int layer, float normalizedTime)
        {
            animator.Play(stateNameHash, 0);
        }

        public override void SyncAnimatorParameter(Operation opt)
        {
            switch (opt.cmd2)
            {
                case 1:
                    parameters[opt.index1].defaultBool = opt.index2 == 1;
                    animator.SetBool(parameters[opt.index1].name, parameters[opt.index1].defaultBool);
                    break;
                case 2:
                    parameters[opt.index1].defaultFloat = opt.direction.x;
                    animator.SetFloat(parameters[opt.index1].name, parameters[opt.index1].defaultFloat);
                    break;
                case 3:
                    parameters[opt.index1].defaultInt = opt.index2;
                    animator.SetInteger(parameters[opt.index1].name, parameters[opt.index1].defaultInt);
                    break;
            }
        }
    }
}
#endif