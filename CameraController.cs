using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * return で視点移動開始
 * escape で終了
 * マウスホイールでズームイン、アウト
 * 
 */
public class CameraController: MonoBehaviour
{
    public GameObject ball;    //中心にするボール
    private Vector3 ballPos;   //ボールの位置

    public float speed = 2.5f;　  //移動スピード調整
    public float distance = 6.0f; //ボールからの距離
    public float zoom = 0.5f;     // ズームスピード

    public Vector2 mouse = Vector2.zero;     //移動の合計距離
    private Vector3 startPos;                //はじめの位置
    private Vector3 zeroPos = Vector3.zero;  //視点移動開始位置

    private float count = 0;       // 視点移動の計算用
    private bool reverse = false;  //視点回転を防ぐ
    private bool enter = false;    // 視点移動画面へ移行

    public bool xRiv = true; // x方向の反転
    private int x;           
    public bool yRiv = false; // y方向の反転
    private int y;

    //float y = Mathf.PI / 2; wiki通りの極座標


    // Start is called before the first frame update
    void Start()
    {
        zeroPos = ballPos + new Vector3(distance, 0, 0);

        startPos = this.transform.position;

        if (xRiv) x = 1;
        else x = -1;
        if (yRiv) y = 1;
        else y = -1;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            enter = true;
            transform.position = zeroPos;
        }
        else if (Input.GetKey(KeyCode.Escape)) enter = false;

        

        if (enter)
        {
            distance += Input.mouseScrollDelta.y * zoom;
            ballPos = ball.transform.position;

            // 移動
            this.transform.position = InputManagement() + ballPos;
            // カメラ反転回避
            AvoidTurn();

            // ボールの方向を向く
            if (reverse) transform.LookAt(ballPos, Vector3.down);
            else transform.LookAt(ballPos);
            

        }
        else // リセット
        {
            transform.position = startPos;
            mouse = Vector2.zero;
            reverse = false;
            count = 0;
            transform.LookAt(ballPos);
        }

    }

    //マウス移動を３D変換、移動距離を返す
    private Vector3 InputManagement()
    {
        //移動パラメータを取得
        if (reverse)
        {
            mouse += new Vector2(-1 * x * Input.GetAxis("Mouse X"),
                y * Input.GetAxis("Mouse Y")) * Time.deltaTime * speed;
        }
        else
        {
            mouse += new Vector2(x * Input.GetAxis("Mouse X"),
                y * Input.GetAxis("Mouse Y")) * Time.deltaTime * speed;
        }

        /*
         * 
         * x = r cos y cos x
         * y = r sin y
         * z = r cos y sin x
         *
         * として球の極座標を求める
         */

        Vector3 pos;

        pos.x = distance * Mathf.Cos(mouse.y) *
           Mathf.Cos(mouse.x);
        pos.y = distance * Mathf.Sin(mouse.y);

        pos.z = distance * Mathf.Cos(mouse.y) *
            Mathf.Sin(mouse.x);

        /*
         * wiki通りの極座標の場合
         */
        //pos.x = distance * Mathf.Sin(y - mouse.y) *
        //   Mathf.Cos(mouse.x);
        //pos.y = distance * Mathf.Cos(y - mouse.y);
        //pos.z = distance * Mathf.Sin(y - mouse.y) *
        //    Mathf.Sin(mouse.x);

        return pos;
    }

    //視点が回転するのを防ぐ
    private void AvoidTurn()
    {
        /*
         * y座標がボールより上か判断
         * パラメータyが pi/2 + n * (2pi) を通り過ぎた瞬間、
         * カメラを反転させ、次の反転場所の判定のため pi を加算または減算する
         */
        if (this.transform.position.y > ballPos.y )
        {
            if (mouse.y > Mathf.PI / 2 + count)
            {
                reverse = true;

                count += Mathf.PI;
            }
            else if (reverse)
            {
                if (mouse.y < Mathf.PI / 2 + count - Mathf.PI)
                {
                    reverse = false;

                    count -= Mathf.PI;
                }
            }
        }
        else if (this.transform.position.y < ballPos.y )
        {
            if (mouse.y > Mathf.PI / 2 + count)
            {
                reverse = false;

                count += Mathf.PI;
            }
            else if (!reverse)
            {
                if (mouse.y < Mathf.PI / 2 + count - Mathf.PI)
                {
                    reverse = true;

                    count -= Mathf.PI;
                }
            }
        }
    }
}
