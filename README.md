# Comments

Comments - це напівфункціональна система коментарів, розроблена з використанням ASP.NET Core для серверної частини. Проєкт надає можливість додавання та відображення  коментарів з підтримкою вкладених відповідей, 
також включає сортування та пагінацію, якщо коротко.

=> Особливості (коротко):
    Це є серверна частина на ASP.NET Core;
    Підтримка вкладених коментарів (дерево коментарів);
    RESTful API для управління коментарями (GET, POST);
    Docker-сумісна архітектура
    Підтримка CORS для взаємодії між клієнтом і сервером


=> Технології, що я використав:
    Backend: ASP.NET Core
    База даних: SQL Server (через взаємодію EF Core)
    Контейнеризація: Docker


=> Запуск із використанням Docker Compose
Клонуйте репозиторій
```bash
git clone https://github.com/ForsakenAnton/Comments.git
cd Comments
```

Побудуйте та запустіть контейнери.

```bash
docker-compose up --build
```

Додаток буде доступний за адресою: http://localhost:5173

Якщо локально, то адреса: https://localhost:5173



=> API Endpoints (та також докладне пояснення, що вміє API)

GET /api/comments/AllCommentsWithChildren - Отримання всіх коментарів з відповідями.
    По дефолту сервер повертає 5 кореневих (не відповідей) з усіма дочірніми коментарями,
    це змінюється за допомогою ?PageSize у рядку запиту [FromQuery].
    Ця кінцева точка зроблена чисто для тестування, у робочому додатку вона несе
    велике навантаження на сервер;

GET /api/comments/ParentComments - Отримання батьківських коментарів 
    Дефолт, як і вище - 5 кореневих з можливістю зміни.

GET /api/comments/GetChildrenComments/{id} - Отримання відповідей на коментар за ID.
    Отримуються тільки коментарі першої вкладеності та скільки 
    кожен з них має дочірніх (кількість).

POST /api/comments - Додавання нового коментаря.
    Тут все цікавіше. За умовою задачі, що форма додавання нового коментаря має мати реєстрацію користувача, також капчу і все інше. Тут мною було прийнято таке рішення, що при кожному додаванні коментаря користувач, звісно, реєструється. В принципі, не те, що реєструється, а вводить свій логін та пошту (навіть не існуючу), також кожен раз вводить капчу. Сама капча генерується та повертається сервером та живе 5 хвилин, асаме живуть сесії, які тримають її значення, далі капча не дійсна, і при невдалій спробі залишити коментар - капча регенерується з другим значенням. Звісно, ми можемо додавати багато коментарів одночасно, і для кожного коментаря буде повністю своя капча, зі своєю сесією. Тут (на сервері) є повна валідація даних, як в самому завданні. Далі, після вдалої спроби залишити коментар - коментар зберігається у БД, також юзер, якщо його ще там нема. Умова додати/недодати - його email. Раптом name/home page будуть другі, а імейл той же - я роблю дуже просто - перезаписую їх та зберігаю дані. 
    Сам метод контролеру викликає кучу сервісів для збереження картинки/текстового файлу (у завданні і сказано що або image або txt, але й чітко не сказано, тому є можливість додати їх обох), виклик HtmlSanitizer для відсіювання лишніх тегів, тощо. Також він скидує сесію з капчою. 
    Ця кінцева точка повертає клієнту доданий у БД коментар.
    Взагалі ідея взята з Reddit, але там просто коментар, далі капча там від google v3 - вона невидима й перевіряє автоматично "людина/бот".

GET /api/comments/captcha - Отримання капчі для достовірності, що юзер - людина.
    Про неї сказано вище. Едине - намальована за допомогою SixLabors.ImageSharp.



=> JSON схема:

```json
{
  "type": "object",
  "properties": {
    "UserGetDto": {
      "type": "object",
      "properties": {
        "Id": { "type": "integer" },
        "UserName": { "type": "string" },
        "Email": { "type": "string" },
        "HomePage": { "type": "string" }
      },
      "required": ["Id", "UserName", "Email", "HomePage"]
    },

    "UserCreateDto": {
      "type": "object",
      "properties": {
        "UserName": {
          "type": "string",
          "pattern": "^[a-zA-Z0-9\\s]+$"
        },
        "Email": { "type": "string", "format": "email" },
        "HomePage": { "type": "string", "format": "uri" }
      },
      "required": ["UserName", "Email"]
    },

    "CommentGetDto": {
      "type": "object",
      "properties": {
        "Id": { "type": "integer" },
        "Text": { "type": "string" },
        "CreationDate": { "type": "string", "format": "date-time" },
        "ImageFileName": { "type": "string" },
        "TextFileName": { "type": "string" },
        "ChildrenCommentsCount": { "type": "integer" },
        "ParentId": { "type": "integer" },
        "UserId": { "type": "integer" },
        "User": {
          "$ref": "#/properties/UserGetDto"
        },
        "Replies": {
          "type": "array",
          "items": { "$ref": "#/properties/CommentGetDto" }
        }
      },
      "required": ["Id", "Text", "CreationDate", "ChildrenCommentsCount", "UserId"]
    },

    "CommentCreateDto": {
      "type": "object",
      "properties": {
        "Text": { "type": "string" },
        "ImageFile": { "type": "string", "format": "binary" },
        "TextFile": { "type": "string", "format": "binary" },
        "Captcha": {
          "type": "string",
          "pattern": "^[a-zA-Z0-9]+$",
          "errorMessage": "CAPTCHA can contain only Latin letters and numbers"
        },
        "ParentId": { "type": "integer" },
        "User": { "$ref": "#/properties/UserCreateDto" }
      },
      "required": ["Text", "Captcha"]
    },

    "CommentParameters": {
      "type": "object",
      "properties": {
        "OrderBy": { "type": "string" },
        "PageNumber": { "type": "integer" },
        "PageSize": { "type": "integer" }
      },
      "required": ["OrderBy", "PageNumber", "PageSize"]
    },

    "RequestParameters": {
      "type": "object",
      "properties": {
        "OrderBy": { "type": "string" },
        "PageNumber": { "type": "integer" },
        "PageSize": { "type": "integer" }
      },
      "required": ["PageNumber", "PageSize"]
    },
    
    "MetaData": {
      "type": "object",
      "properties": {
        "CurrentPage": { "type": "integer" },
        "TotalPages": { "type": "integer" },
        "PageSize": { "type": "integer" },
        "TotalCount": { "type": "integer" },
        "HasPrevious": { "type": "boolean" },
        "HasNext": { "type": "boolean" }
      },
      "required": ["CurrentPage", "TotalPages", "PageSize", "TotalCount"]
    }
  }
}
```



=> Повний API документ для імпорту, наприклад, у Postman:

```json
{
  "openapi": "3.0.1",
  "info": {
    "title": "Comments.Server | v1",
    "version": "1.0.0"
  },
  "servers": [
    {
      "url": "https://localhost:7092/"
    }
  ],
  "paths": {
    "/api/comments/AllCommentsWithChildren": {
      "get": {
        "tags": [
          "Comments"
        ],
        "parameters": [
          {
            "name": "OrderBy",
            "in": "query",
            "schema": {
              "pattern": "^[a-zA-Z0-9\\s]+$",
              "type": "string",
              "format": "uri"
            }
          },
          {
            "name": "PageNumber",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "PageSize",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/comments/ParentComments": {
      "get": {
        "tags": [
          "Comments"
        ],
        "parameters": [
          {
            "name": "OrderBy",
            "in": "query",
            "schema": {
              "pattern": "^[a-zA-Z0-9\\s]+$",
              "type": "string",
              "format": "uri"
            }
          },
          {
            "name": "PageNumber",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          },
          {
            "name": "PageSize",
            "in": "query",
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/comments/GetChildrenComments/{id}": {
      "get": {
        "tags": [
          "Comments"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CommentGetDto"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/comments/captcha/{parentCommentId}": {
      "get": {
        "tags": [
          "Comments"
        ],
        "parameters": [
          {
            "name": "parentCommentId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/comments": {
      "post": {
        "tags": [
          "Comments"
        ],
        "requestBody": {
          "content": {
            "application/x-www-form-urlencoded": {
              "schema": {
                "type": "object",
                "properties": {
                  "Text": {
                    "pattern": "^[a-zA-Z0-9\\s]+$",
                    "type": "string",
                    "format": "uri"
                  },
                  "ImageFile": {
                    "$ref": "#/components/schemas/IFormFile"
                  },
                  "TextFile": {
                    "$ref": "#/components/schemas/IFormFile"
                  },
                  "Captcha": {
                    "pattern": "^[a-zA-Z0-9]+$",
                    "type": "string",
                    "format": "uri"
                  },
                  "ParentId": {
                    "type": "integer",
                    "format": "int32"
                  },
                  "User.UserName": {
                    "pattern": "^[a-zA-Z0-9\\s]+$",
                    "type": "string",
                    "format": "uri"
                  },
                  "User.Email": {
                    "pattern": "^[a-zA-Z0-9\\s]+$",
                    "type": "string",
                    "format": "uri"
                  },
                  "User.HomePage": {
                    "pattern": "^[a-zA-Z0-9\\s]+$",
                    "type": "string",
                    "format": "uri"
                  }
                }
              }
            }
          },
          "required": true
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "CommentGetDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "text": {
            "type": "string"
          },
          "creationDate": {
            "type": "string",
            "format": "date-time"
          },
          "imageFileName": {
            "type": "string",
            "nullable": true
          },
          "textFileName": {
            "type": "string",
            "nullable": true
          },
          "childrenCommentsCount": {
            "type": "integer",
            "format": "int32"
          },
          "parentId": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "userId": {
            "type": "integer",
            "format": "int32"
          },
          "user": {
            "$ref": "#/components/schemas/UserGetDto"
          },
          "replies": {
            "type": "array",
            "items": { }
          }
        }
      },
      "IFormFile": {
        "type": "string",
        "format": "binary"
      },
      "UserGetDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "userName": {
            "type": "string"
          },
          "email": {
            "type": "string"
          },
          "homePage": {
            "type": "string"
          }
        },
        "nullable": true
      }
    }
  },
  "tags": [
    {
      "name": "Comments"
    }
  ]
}
```
